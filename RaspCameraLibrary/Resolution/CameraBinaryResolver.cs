using RaspCameraLibrary.Diagnostics;

namespace RaspCameraLibrary.Resolution;

/// <summary>
/// Resolves rpicam-* binaries with fallback to legacy libcamera-* names.
/// </summary>
public static class CameraBinaryResolver
{
    private static readonly Dictionary<CameraTool, string[]> Candidates = new()
    {
        [CameraTool.Hello] = ["rpicam-hello", "libcamera-hello"],
        [CameraTool.Vid] = ["rpicam-vid", "libcamera-vid"],
        [CameraTool.Still] = ["rpicam-still", "libcamera-still"]
    };

    private static readonly Dictionary<CameraTool, string> Cache = new();
    private static readonly object Sync = new();

    /// <summary>
    /// Prefer modern rpicam names when both exist.
    /// </summary>
    public static bool PreferRpicam { get; set; } = true;

    /// <summary>
    /// Clear cached resolutions (useful in tests).
    /// </summary>
    public static void ClearCache()
    {
        lock (Sync)
        {
            Cache.Clear();
        }
    }

    /// <summary>
    /// Candidate binary names for a tool, ordered by preference.
    /// </summary>
    public static IReadOnlyList<string> GetCandidates(CameraTool tool)
    {
        var names = Candidates[tool];
        if (PreferRpicam)
        {
            return names;
        }

        return names.Reverse().ToArray();
    }

    /// <summary>
    /// Resolve executable name available on PATH.
    /// </summary>
    public static string Resolve(CameraTool tool)
    {
        lock (Sync)
        {
            if (Cache.TryGetValue(tool, out var cached))
            {
                return cached;
            }

            var tried = GetCandidates(tool);
            foreach (var name in tried)
            {
                if (IsOnPath(name))
                {
                    Cache[tool] = name;
                    return name;
                }
            }

            throw new CameraToolNotFoundException(tool, tried);
        }
    }

    /// <summary>
    /// Try resolve without throwing.
    /// </summary>
    public static bool TryResolve(CameraTool tool, out string? executable)
    {
        try
        {
            executable = Resolve(tool);
            return true;
        }
        catch (CameraToolNotFoundException)
        {
            executable = null;
            return false;
        }
    }

    /// <summary>
    /// Check whether an executable name exists on PATH.
    /// </summary>
    public static bool IsOnPath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        if (Path.IsPathRooted(fileName))
        {
            return File.Exists(fileName);
        }

        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        var directories = pathEnv.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        var extensions = GetExecutableExtensions();

        foreach (var directory in directories)
        {
            foreach (var extension in extensions)
            {
                var candidate = Path.Combine(directory, fileName + extension);
                if (File.Exists(candidate))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static string[] GetExecutableExtensions()
    {
        if (!OperatingSystem.IsWindows())
        {
            return [string.Empty];
        }

        var pathext = Environment.GetEnvironmentVariable("PATHEXT") ?? ".EXE;.CMD;.BAT";
        return pathext.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(e => e.StartsWith('.') ? e : "." + e)
            .Prepend(string.Empty)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
