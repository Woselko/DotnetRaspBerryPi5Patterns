using RaspCameraLibrary.Diagnostics;
using RaspCameraLibrary.Sessions;
using RaspCameraLibrary.Settings;

namespace RaspCameraLibrary.MultiCamera;

/// <summary>
/// Parallel capture helpers for multiple camera indexes.
/// </summary>
public static class MultiCameraCapture
{
    /// <summary>
    /// Capture video from multiple cameras in parallel.
    /// </summary>
    public static async Task CaptureVideosAsync(
        IEnumerable<VideoSettings> settingsList,
        CancellationToken cancellationToken = default,
        int maxDegreeOfParallelism = 2)
    {
        ArgumentNullException.ThrowIfNull(settingsList);

        var settings = settingsList.ToList();
        if (settings.Count == 0)
        {
            return;
        }

        if (maxDegreeOfParallelism < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism), "Must be >= 1.");
        }

        ValidateDistinctCameras(settings.Select(s => s.Camera));

        using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism);
        var tasks = settings.Select(async item =>
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var startInfo = Video.CaptureStartInfo(item);
                await using var session = await CameraProcessSession.StartAsync(startInfo, cancellationToken).ConfigureAwait(false);
                await session.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    /// <summary>
    /// Capture stills from multiple cameras in parallel.
    /// </summary>
    public static async Task CaptureStillsAsync(
        IEnumerable<StillSettings> settingsList,
        CancellationToken cancellationToken = default,
        int maxDegreeOfParallelism = 2)
    {
        ArgumentNullException.ThrowIfNull(settingsList);

        var settings = settingsList.ToList();
        if (settings.Count == 0)
        {
            return;
        }

        if (maxDegreeOfParallelism < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism), "Must be >= 1.");
        }

        ValidateDistinctCameras(settings.Select(s => s.Camera));

        using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism);
        var tasks = settings.Select(async item =>
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var startInfo = Still.CaptureStartInfo(item);
                await using var session = await CameraProcessSession.StartAsync(startInfo, cancellationToken).ConfigureAwait(false);
                await session.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    /// <summary>
    /// Convenience: record H264 from camera 0 and 1 for the same duration.
    /// </summary>
    public static Task RecordBothH264Async(
        string outputCamera0,
        string outputCamera1,
        uint durationMs = 5000,
        uint width = 1280,
        uint height = 720,
        CancellationToken cancellationToken = default)
    {
        var settings = new VideoSettings[]
        {
            VideoSettings.H264(Camera: 0, Width: width, Height: height, Timeout: durationMs, HFlip: false, VFlip: false, Output: outputCamera0),
            VideoSettings.H264(Camera: 1, Width: width, Height: height, Timeout: durationMs, HFlip: false, VFlip: false, Output: outputCamera1)
        };

        return CaptureVideosAsync(settings, cancellationToken, maxDegreeOfParallelism: 2);
    }

    private static void ValidateDistinctCameras(IEnumerable<uint?> cameraIds)
    {
        var ids = cameraIds.Select(id => id ?? 0).ToList();
        if (ids.Distinct().Count() != ids.Count)
        {
            throw new CameraException("Each parallel capture must use a distinct camera id.");
        }
    }
}
