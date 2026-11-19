using System.Diagnostics;
using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Types;

namespace RaspCameraLibrary;

/// <summary>
/// rpicam-vid Wrapper
/// </summary>
public class Video : Hello
{
    /// <summary>
    /// Camera CLI tool
    /// </summary>
    protected override CameraTool Tool => CameraTool.Vid;

    private static Video Instance { get; } = new Video();

    /// <summary>
    /// Generate a ProcessStartInfo for rpicam-vid
    /// </summary>
    public static ProcessStartInfo CaptureStartInfo(VideoSettings settings) => Instance.StartInfo(settings);

    /// <summary>
    /// List cameras
    /// </summary>
    public static new async Task<List<Camera>?> ListCameras() => await Instance.List();
}
