namespace RaspCameraLibrary.Diagnostics;

/// <summary>
/// Base exception for camera library failures.
/// </summary>
public class CameraException : Exception
{
    public CameraException(string message) : base(message)
    {
    }

    public CameraException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Thrown when rpicam/libcamera binaries are missing from PATH.
/// </summary>
public sealed class CameraToolNotFoundException : CameraException
{
    public CameraTool Tool { get; }
    public IReadOnlyList<string> TriedBinaries { get; }

    public CameraToolNotFoundException(CameraTool tool, IReadOnlyList<string> tried)
        : base($"Camera tool '{tool}' was not found. Tried: {string.Join(", ", tried)}. Install rpicam-apps (or legacy libcamera-apps).")
    {
        Tool = tool;
        TriedBinaries = tried;
    }
}

/// <summary>
/// Thrown when no camera device is available or the requested id is missing.
/// </summary>
public sealed class CameraNotFoundException : CameraException
{
    public uint? CameraId { get; }

    public CameraNotFoundException(string message, uint? cameraId = null) : base(message)
    {
        CameraId = cameraId;
    }
}

/// <summary>
/// Thrown when a camera process fails to start or exits with an error.
/// </summary>
public sealed class CameraProcessException : CameraException
{
    public string? FileName { get; }
    public string? Arguments { get; }
    public int? ExitCode { get; }
    public string? StdError { get; }

    public CameraProcessException(
        string message,
        string? fileName = null,
        string? arguments = null,
        int? exitCode = null,
        string? stdError = null)
        : base(BuildMessage(message, stdError))
    {
        FileName = fileName;
        Arguments = arguments;
        ExitCode = exitCode;
        StdError = stdError;
    }

    public CameraProcessException(
        string message,
        Exception innerException,
        string? fileName = null,
        string? arguments = null,
        int? exitCode = null,
        string? stdError = null)
        : base(BuildMessage(message, stdError), innerException)
    {
        FileName = fileName;
        Arguments = arguments;
        ExitCode = exitCode;
        StdError = stdError;
    }

    private static string BuildMessage(string message, string? stdError)
    {
        if (string.IsNullOrWhiteSpace(stdError))
        {
            return message;
        }

        return $"{message} stderr: {stdError.Trim()}";
    }
}

/// <summary>
/// Thrown when capture output path/options are invalid.
/// </summary>
public sealed class CaptureOutputException : CameraException
{
    public CaptureOutputException(string message) : base(message)
    {
    }
}
