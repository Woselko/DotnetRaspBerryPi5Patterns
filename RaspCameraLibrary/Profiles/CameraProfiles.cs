using RaspCameraLibrary.Output;
using RaspCameraLibrary.Services;
using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Codecs;
using RaspCameraLibrary.Settings.Encodings;

namespace RaspCameraLibrary.Profiles;

/// <summary>
/// Ready-made capture recipes matching common rpicam CLI usage.
/// </summary>
public static class CameraProfiles
{
    private static readonly CameraService DefaultService = new();

    /// <summary>
    /// rpicam-hello --camera {id} -t {timeoutMs}
    /// </summary>
    public static Task QuickPreviewAsync(
        uint cameraId = 0,
        uint timeoutMs = 5000,
        CancellationToken cancellationToken = default,
        ICameraService? cameraService = null)
        => (cameraService ?? DefaultService).PreviewAsync(cameraId, timeoutMs, cancellationToken);

    /// <summary>
    /// rpicam-vid --camera {id} -t {durationMs} -o {outputPath} (H.264)
    /// </summary>
    public static Task RecordH264Async(
        string outputPath,
        uint cameraId = 0,
        uint durationMs = 5000,
        uint width = 1280,
        uint height = 720,
        CancellationToken cancellationToken = default,
        ICameraService? cameraService = null)
    {
        var settings = VideoSettings.H264(
            Camera: cameraId,
            Width: width,
            Height: height,
            Timeout: durationMs,
            HFlip: false,
            VFlip: false,
            Output: outputPath);

        CaptureOutput.File(outputPath).Validate();
        return (cameraService ?? DefaultService).CaptureVideoAsync(settings, cancellationToken);
    }

    /// <summary>
    /// rpicam-still --camera {id} -o {outputPath} (JPEG)
    /// </summary>
    public static Task SnapshotJpgAsync(
        string outputPath,
        uint cameraId = 0,
        uint width = 1920,
        uint height = 1080,
        uint timeoutMs = 1000,
        CancellationToken cancellationToken = default,
        ICameraService? cameraService = null)
    {
        var settings = StillSettings.Jpg(
            Camera: cameraId,
            Width: width,
            Height: height,
            Timeout: timeoutMs,
            HFlip: false,
            VFlip: false,
            Immediate: true,
            Output: outputPath);

        CaptureOutput.File(outputPath).Validate();
        return (cameraService ?? DefaultService).CaptureStillAsync(settings, cancellationToken);
    }

    /// <summary>
    /// Build H264 settings for the given camera/duration/path without starting capture.
    /// </summary>
    public static H264 CreateH264Settings(
        string outputPath,
        uint cameraId = 0,
        uint durationMs = 5000,
        uint width = 1280,
        uint height = 720)
        => VideoSettings.H264(
            Camera: cameraId,
            Width: width,
            Height: height,
            Timeout: durationMs,
            HFlip: false,
            VFlip: false,
            Output: outputPath);

    /// <summary>
    /// Build JPEG still settings without starting capture.
    /// </summary>
    public static Jpg CreateJpgSettings(
        string outputPath,
        uint cameraId = 0,
        uint width = 1920,
        uint height = 1080,
        uint timeoutMs = 1000)
        => StillSettings.Jpg(
            Camera: cameraId,
            Width: width,
            Height: height,
            Timeout: timeoutMs,
            HFlip: false,
            VFlip: false,
            Immediate: true,
            Output: outputPath);
}
