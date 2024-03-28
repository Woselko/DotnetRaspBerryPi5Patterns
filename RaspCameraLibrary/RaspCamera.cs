using RaspCameraLibrary.Helpers;
using System.Diagnostics;

namespace RaspCameraLibrary;

/// <summary>
/// RaspCamera base class for binaries
/// </summary>
public abstract class RaspCamera
{
    /// <summary>
    /// Binary name
    /// </summary>
    protected abstract string Executable { get; }

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
        UseShellExecute = false
    };

    /// <summary>
    /// Generate start info from arguments
    /// </summary>x
    protected ProcessStartInfo StartInfo(Arguments arguments) => new()
    {
        FileName = Executable,
        Arguments = arguments.ToString(),
        RedirectStandardInput = true,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        UseShellExecute = false
    };
}
