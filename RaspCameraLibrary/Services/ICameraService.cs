using RaspCameraLibrary.Diagnostics;
using RaspCameraLibrary.Output;
using RaspCameraLibrary.Sessions;
using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Types;

namespace RaspCameraLibrary.Services;

/// <summary>
/// High-level camera operations suitable for DI.
/// </summary>
public interface ICameraService
{
    Task<CameraHealthReport> CheckHealthAsync(bool listCameras = true, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Camera>> GetCamerasAsync(CancellationToken cancellationToken = default);

    Task<Camera> GetCameraAsync(uint cameraId, CancellationToken cancellationToken = default);

    Task CaptureVideoAsync(VideoSettings settings, CancellationToken cancellationToken = default);

    Task CaptureStillAsync(StillSettings settings, CancellationToken cancellationToken = default);

    Task PreviewAsync(uint cameraId = 0, uint timeoutMs = 5000, CancellationToken cancellationToken = default);

    Task CaptureVideoAsync(
        VideoSettings settings,
        CaptureOutput output,
        CancellationToken cancellationToken = default);

    Task CaptureStillAsync(
        StillSettings settings,
        CaptureOutput output,
        CancellationToken cancellationToken = default);

    Task<CameraProcessSession> StartVideoSessionAsync(
        VideoSettings settings,
        CancellationToken cancellationToken = default);
}
