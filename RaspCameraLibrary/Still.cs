using System.Diagnostics;
using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Types;

namespace RaspCameraLibrary;

/// <summary>
/// rpicam-still Wrapper
/// </summary>
public class Still : Hello
{
    /// <summary>
    /// Camera CLI tool
    /// </summary>
    protected override CameraTool Tool => CameraTool.Still;

    private static Still Instance { get; } = new Still();

    /// <summary>
    /// Generate a ProcessStartInfo for rpicam-still
    /// </summary>
    public static ProcessStartInfo CaptureStartInfo(StillSettings settings) => Instance.StartInfo(settings);

    /// <summary>
    /// List cameras
    /// </summary>
    public static new async Task<List<Camera>?> ListCameras() => await Instance.List();
}
