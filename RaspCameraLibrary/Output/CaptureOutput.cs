using RaspCameraLibrary.Diagnostics;
using RaspCameraLibrary.Settings;

namespace RaspCameraLibrary.Output;

/// <summary>
/// Capture destination kinds.
/// </summary>
public enum CaptureOutputKind
{
    File,
    Stdout,
    NamedPipe
}

/// <summary>
/// Output target for rpicam capture (--output).
/// </summary>
public sealed class CaptureOutput
{
    private CaptureOutput(CaptureOutputKind kind, string value)
    {
        Kind = kind;
        Value = value;
    }

    public CaptureOutputKind Kind { get; }
    public string Value { get; }

    public static CaptureOutput File(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new CaptureOutputException("Output file path is required.");
        }

        return new CaptureOutput(CaptureOutputKind.File, path);
    }

    public static CaptureOutput Stdout { get; } = new(CaptureOutputKind.Stdout, "/dev/stdout");

    public static CaptureOutput NamedPipe(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new CaptureOutputException("Named pipe path is required.");
        }

        return new CaptureOutput(CaptureOutputKind.NamedPipe, path);
    }

    /// <summary>
    /// Validate path constraints before starting capture.
    /// </summary>
    public void Validate()
    {
        switch (Kind)
        {
            case CaptureOutputKind.Stdout:
                return;

            case CaptureOutputKind.File:
            {
                var directory = Path.GetDirectoryName(Path.GetFullPath(Value));
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    throw new CaptureOutputException($"Output directory does not exist: {directory}");
                }

                break;
            }

            case CaptureOutputKind.NamedPipe:
            {
                if (OperatingSystem.IsWindows())
                {
                    if (!Value.StartsWith(@"\\.\pipe\", StringComparison.OrdinalIgnoreCase)
                        && !Value.StartsWith("/tmp/", StringComparison.OrdinalIgnoreCase))
                    {
                        // Allow unix-style paths when running under WSL-like environments.
                    }
                }

                var directory = Path.GetDirectoryName(Path.GetFullPath(Value));
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    throw new CaptureOutputException($"Named pipe directory does not exist: {directory}");
                }

                break;
            }
        }
    }

    public void Apply(VideoSettings settings)
    {
        Validate();
        settings.Output = Value;
    }

    public void Apply(StillSettings settings)
    {
        Validate();
        settings.Output = Value;
    }

    public override string ToString() => Value;
}
