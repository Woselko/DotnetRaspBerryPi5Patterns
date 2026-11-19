using RaspCameraLibrary.Diagnostics;
using RaspCameraLibrary.Settings.Types;

namespace RaspCameraLibrary;

/// <summary>
/// Discovers and selects connected cameras.
/// </summary>
public static class CameraSelector
{
    /// <summary>
    /// List connected cameras (rpicam-hello --list-cameras).
    /// </summary>
    public static async Task<IReadOnlyList<Camera>> GetCamerasAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var cameras = await Hello.ListCameras().ConfigureAwait(false);
        if (cameras is null)
        {
            throw new CameraProcessException("Failed to list cameras. Is rpicam-hello available and the camera enabled?");
        }

        return cameras;
    }

    /// <summary>
    /// Get a camera by id or throw.
    /// </summary>
    public static async Task<Camera> GetCameraAsync(uint cameraId, CancellationToken cancellationToken = default)
    {
        var cameras = await GetCamerasAsync(cancellationToken).ConfigureAwait(false);
        var camera = cameras.FirstOrDefault(c => c.Id == cameraId);
        if (camera is null)
        {
            throw new CameraNotFoundException($"Camera with id {cameraId} was not found.", cameraId);
        }

        return camera;
    }

    /// <summary>
    /// Get the first available camera or throw.
    /// </summary>
    public static async Task<Camera> GetDefaultCameraAsync(CancellationToken cancellationToken = default)
    {
        var cameras = await GetCamerasAsync(cancellationToken).ConfigureAwait(false);
        if (cameras.Count == 0)
        {
            throw new CameraNotFoundException("No cameras were detected.");
        }

        return cameras[0];
    }

    /// <summary>
    /// Try get camera by id.
    /// </summary>
    public static async Task<Camera?> TryGetCameraAsync(uint cameraId, CancellationToken cancellationToken = default)
    {
        var cameras = await GetCamerasAsync(cancellationToken).ConfigureAwait(false);
        return cameras.FirstOrDefault(c => c.Id == cameraId);
    }
}
