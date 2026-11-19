using RaspCameraLibrary.Diagnostics;
using RaspCameraLibrary.Output;
using RaspCameraLibrary.Sessions;
using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Types;

namespace RaspCameraLibrary.Services;

/// <summary>
/// Default <see cref="ICameraService"/> implementation.
/// </summary>
public sealed class CameraService : ICameraService
{
    public Task<CameraHealthReport> CheckHealthAsync(
        bool listCameras = true,
        CancellationToken cancellationToken = default)
        => CameraDiagnostics.CheckAsync(listCameras, cancellationToken);

    public Task<IReadOnlyList<Camera>> GetCamerasAsync(CancellationToken cancellationToken = default)
        => CameraSelector.GetCamerasAsync(cancellationToken);

    public Task<Camera> GetCameraAsync(uint cameraId, CancellationToken cancellationToken = default)
        => CameraSelector.GetCameraAsync(cameraId, cancellationToken);

    public async Task CaptureVideoAsync(VideoSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        await using var session = await StartVideoSessionAsync(settings, cancellationToken).ConfigureAwait(false);

        if (settings.Timeout is > 0)
        {
            await session.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        await session.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task CaptureStillAsync(StillSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var startInfo = Still.CaptureStartInfo(settings);
        await using var session = await CameraProcessSession.StartAsync(startInfo, cancellationToken).ConfigureAwait(false);
        await session.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task PreviewAsync(
        uint cameraId = 0,
        uint timeoutMs = 5000,
        CancellationToken cancellationToken = default)
    {
        var startInfo = Hello.PreviewStartInfo(cameraId, timeoutMs);
        await using var session = await CameraProcessSession.StartAsync(startInfo, cancellationToken).ConfigureAwait(false);

        if (timeoutMs == 0)
        {
            await session.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        await session.RunForAsync(TimeSpan.FromMilliseconds(timeoutMs + 2000), cancellationToken).ConfigureAwait(false);
    }

    public Task CaptureVideoAsync(
        VideoSettings settings,
        CaptureOutput output,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(output);
        output.Apply(settings);
        return CaptureVideoAsync(settings, cancellationToken);
    }

    public Task CaptureStillAsync(
        StillSettings settings,
        CaptureOutput output,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(output);
        output.Apply(settings);
        return CaptureStillAsync(settings, cancellationToken);
    }

    public Task<CameraProcessSession> StartVideoSessionAsync(
        VideoSettings settings,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var startInfo = Video.CaptureStartInfo(settings);
        return CameraProcessSession.StartAsync(startInfo, cancellationToken);
    }
}
