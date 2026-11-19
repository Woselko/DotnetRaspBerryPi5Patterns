using System.Diagnostics;
using RaspCameraLibrary.Helpers;
using RaspCameraLibrary.Resolution;

namespace RaspCameraLibrary;

/// <summary>
/// RaspCamera base class for binaries
/// </summary>
public abstract class RaspCamera
{
    /// <summary>
    /// Camera CLI tool kind.
    /// </summary>
    protected abstract CameraTool Tool { get; }

    /// <summary>
    /// Resolved binary name (rpicam-* with libcamera-* fallback).
    /// </summary>
    protected virtual string Executable => CameraBinaryResolver.Resolve(Tool);

    /// <summary>
    /// Generate start info from string arguments
    /// </summary>
    protected ProcessStartInfo StartInfo(string arguments) => new()
    {
        FileName = Executable,
        Arguments = arguments,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    /// <summary>
    /// Generate start info from arguments
    /// </summary>
    protected ProcessStartInfo StartInfo(Arguments arguments) => new()
    {
        FileName = Executable,
        Arguments = arguments.ToString(),
        RedirectStandardInput = true,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };
}
