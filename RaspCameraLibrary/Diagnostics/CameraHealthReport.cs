namespace RaspCameraLibrary.Diagnostics;

/// <summary>
/// Result of camera environment diagnostics.
/// </summary>
public sealed class CameraHealthReport
{
    public required bool IsHealthy { get; init; }
    public required bool HelloAvailable { get; init; }
    public required bool VidAvailable { get; init; }
    public required bool StillAvailable { get; init; }
    public string? HelloBinary { get; init; }
    public string? VidBinary { get; init; }
    public string? StillBinary { get; init; }
    public int CameraCount { get; init; }
    public IReadOnlyList<string> Messages { get; init; } = [];
}

/// <summary>
/// Probes PATH and optionally lists cameras.
/// </summary>
public static class CameraDiagnostics
{
    /// <summary>
    /// Check binaries on PATH. Optionally probe connected cameras.
    /// </summary>
    public static async Task<CameraHealthReport> CheckAsync(bool listCameras = true, CancellationToken cancellationToken = default)
    {
        var messages = new List<string>();

        var helloOk = Resolution.CameraBinaryResolver.TryResolve(CameraTool.Hello, out var hello);
        var vidOk = Resolution.CameraBinaryResolver.TryResolve(CameraTool.Vid, out var vid);
        var stillOk = Resolution.CameraBinaryResolver.TryResolve(CameraTool.Still, out var still);

        if (!helloOk) messages.Add("rpicam-hello/libcamera-hello not found on PATH.");
        if (!vidOk) messages.Add("rpicam-vid/libcamera-vid not found on PATH.");
        if (!stillOk) messages.Add("rpicam-still/libcamera-still not found on PATH.");

        var cameraCount = 0;
        if (listCameras && helloOk)
        {
            try
            {
                var cameras = await CameraSelector.GetCamerasAsync(cancellationToken);
                cameraCount = cameras.Count;
                if (cameraCount == 0)
                {
                    messages.Add("Camera tools are present but no cameras were detected.");
                }
            }
            catch (Exception ex)
            {
                messages.Add($"Failed to list cameras: {ex.Message}");
            }
        }

        var healthy = helloOk && vidOk && stillOk && (!listCameras || cameraCount > 0);

        if (healthy)
        {
            messages.Add("Camera environment looks OK.");
        }

        return new CameraHealthReport
        {
            IsHealthy = healthy,
            HelloAvailable = helloOk,
            VidAvailable = vidOk,
            StillAvailable = stillOk,
            HelloBinary = hello,
            VidBinary = vid,
            StillBinary = still,
            CameraCount = cameraCount,
            Messages = messages
        };
    }
}
