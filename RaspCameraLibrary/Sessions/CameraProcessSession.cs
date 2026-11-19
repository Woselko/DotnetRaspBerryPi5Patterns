using System.Diagnostics;
using System.Text;
using RaspCameraLibrary.Diagnostics;

namespace RaspCameraLibrary.Sessions;

/// <summary>
/// Managed camera CLI process with safe cleanup.
/// </summary>
public sealed class CameraProcessSession : IAsyncDisposable
{
    private readonly StringBuilder _stdError = new();
    private bool _disposed;

    private CameraProcessSession(System.Diagnostics.Process process)
    {
        Process = process;
    }

    public System.Diagnostics.Process Process { get; }

    public int Id => Process.Id;

    public bool HasExited => Process.HasExited;

    public string StandardErrorText
    {
        get
        {
            lock (_stdError)
            {
                return _stdError.ToString();
            }
        }
    }

    /// <summary>
    /// Start a camera process from prepared start info.
    /// </summary>
    public static Task<CameraProcessSession> StartAsync(
        ProcessStartInfo startInfo,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardError = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardInput = true;
        startInfo.CreateNoWindow = true;

        System.Diagnostics.Process process;
        try
        {
            process = System.Diagnostics.Process.Start(startInfo)
                ?? throw new CameraProcessException(
                    "Failed to start camera process (Process.Start returned null).",
                    startInfo.FileName,
                    startInfo.Arguments);
        }
        catch (Exception ex) when (ex is not CameraProcessException)
        {
            throw new CameraProcessException(
                $"Failed to start camera process '{startInfo.FileName}'.",
                ex,
                startInfo.FileName,
                startInfo.Arguments);
        }

        var session = new CameraProcessSession(process);
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is null) return;
            lock (session._stdError)
            {
                session._stdError.AppendLine(e.Data);
            }
        };
        process.BeginErrorReadLine();

        return Task.FromResult(session);
    }

    /// <summary>
    /// Wait until the process exits. Cancelling kills the process.
    /// </summary>
    public async Task WaitForExitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await Process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            await KillAsync().ConfigureAwait(false);
            throw;
        }

        if (Process.ExitCode != 0)
        {
            throw new CameraProcessException(
                $"Camera process exited with code {Process.ExitCode}.",
                Process.StartInfo.FileName,
                Process.StartInfo.Arguments,
                Process.ExitCode,
                StandardErrorText);
        }
    }

    /// <summary>
    /// Run until duration elapses (then kill) or process exits earlier.
    /// </summary>
    public async Task RunForAsync(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(duration);

        try
        {
            await Process.WaitForExitAsync(timeoutCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            await KillAsync().ConfigureAwait(false);
            return;
        }
        catch (OperationCanceledException)
        {
            await KillAsync().ConfigureAwait(false);
            throw;
        }

        if (Process.ExitCode != 0)
        {
            throw new CameraProcessException(
                $"Camera process exited with code {Process.ExitCode}.",
                Process.StartInfo.FileName,
                Process.StartInfo.Arguments,
                Process.ExitCode,
                StandardErrorText);
        }
    }

    public async Task KillAsync()
    {
        if (Process.HasExited)
        {
            return;
        }

        try
        {
            Process.Kill(entireProcessTree: true);
            await Process.WaitForExitAsync().ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
            // already exited
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        await KillAsync().ConfigureAwait(false);
        Process.Dispose();
    }
}
