using System.Runtime.CompilerServices;
using System.Threading.Channels;
using RaspCameraLibrary.Output;
using RaspCameraLibrary.Settings;

namespace RaspCameraLibrary.Streaming;

/// <summary>
/// Captures MJPEG frames from rpicam-vid and exposes them as events or async stream.
/// </summary>
public sealed class MjpegFrameSource : IAsyncDisposable
{
    private readonly VideoStream _stream = new();
    private readonly Channel<byte[]> _frames = Channel.CreateBounded<byte[]>(
        new BoundedChannelOptions(8)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = false,
            SingleWriter = true
        });

    private CancellationTokenSource? _cts;
    private Task? _readerTask;
    private bool _disposed;

    public event Action<byte[]>? FrameReceived;

    public bool IsRunning => _readerTask is { IsCompleted: false };

    /// <summary>
    /// Start MJPEG capture to stdout.
    /// </summary>
    public Task StartAsync(
        uint cameraId = 0,
        uint width = 1280,
        uint height = 720,
        uint framerate = 15,
        CancellationToken cancellationToken = default)
    {
        var settings = VideoSettings.Mjpeg(
            Camera: cameraId,
            Width: width,
            Height: height,
            Timeout: 0,
            HFlip: false,
            VFlip: false,
            Framerate: framerate,
            Output: CaptureOutput.Stdout.Value);
        settings.Flush = true;

        return StartAsync(settings, cancellationToken);
    }

    public Task StartAsync(VideoSettings settings, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(settings);

        if (IsRunning)
        {
            throw new InvalidOperationException("MJPEG source is already running.");
        }

        settings.Output ??= CaptureOutput.Stdout.Value;
        settings.Flush ??= true;

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var startInfo = VideoStream.CaptureStartInfo(settings);

        _stream.NewImageReceived += OnFrame;
        _readerTask = Task.Run(
            () => _stream.StartFrameReaderAsync(startInfo, _cts.Token),
            CancellationToken.None);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Async stream of JPEG frames.
    /// </summary>
    public async IAsyncEnumerable<byte[]> ReadFramesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await foreach (var frame in _frames.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            yield return frame;
        }
    }

    /// <summary>
    /// Pump frames into an HTTP multipart response stream.
    /// </summary>
    public async Task WriteToHttpStreamAsync(Stream responseBody, CancellationToken cancellationToken = default)
    {
        await foreach (var frame in ReadFramesAsync(cancellationToken).ConfigureAwait(false))
        {
            await MjpegHttpWriter.WriteFrameAsync(responseBody, frame, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task StopAsync()
    {
        if (_cts is null)
        {
            return;
        }

        await _cts.CancelAsync().ConfigureAwait(false);

        try
        {
            await _stream.StopVideoStream().ConfigureAwait(false);
        }
        catch
        {
            // ignore shutdown races
        }

        if (_readerTask is not null)
        {
            try
            {
                await _readerTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        _stream.NewImageReceived -= OnFrame;
        _frames.Writer.TryComplete();
        _cts.Dispose();
        _cts = null;
        _readerTask = null;
    }

    private void OnFrame(byte[] frame)
    {
        FrameReceived?.Invoke(frame);
        _frames.Writer.TryWrite(frame);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        await StopAsync().ConfigureAwait(false);
    }
}
