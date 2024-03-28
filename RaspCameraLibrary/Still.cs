using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Types;
using System.Diagnostics;

namespace RaspCameraLibrary;

/// <summary>
/// libcamera-still Wrapper
/// </summary>
public class Still : Hello
{
    /// <summary>
    /// Binary name
    /// </summary>
    protected override string Executable => "libcamera-still";

    private static Still Instance { get; } = new Still();

    /// <summary>
    /// Generate a ProcessStartInfo for libcamera-still
    /// </summary>
    public static ProcessStartInfo CaptureStartInfo(StillSettings settings) => Instance.StartInfo(settings);

    /// <summary>
    /// List cameras
    /// </summary>
    public static new async Task<List<Camera>?> ListCameras() => await Instance.List();
}
