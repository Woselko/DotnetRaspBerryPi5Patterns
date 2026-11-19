using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Enumerations;

namespace RaspCameraLibrary.Metadata;

/// <summary>
/// Helpers for EXIF / pts / metadata sidecar options.
/// </summary>
public static class CaptureMetadata
{
    /// <summary>
    /// Apply video metadata options (pts file and/or metadata dump).
    /// </summary>
    public static VideoSettings Apply(
        VideoSettings settings,
        string? savePtsPath = null,
        string? metadataPath = null,
        MetadataFormat? metadataFormat = null)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (!string.IsNullOrWhiteSpace(savePtsPath))
        {
            settings.SavePts = savePtsPath;
        }

        if (!string.IsNullOrWhiteSpace(metadataPath))
        {
            settings.Metadata = metadataPath;
        }

        if (metadataFormat is not null)
        {
            settings.MetadataFormat = metadataFormat;
        }

        return settings;
    }

    /// <summary>
    /// Apply still metadata options (metadata dump and/or EXIF string).
    /// </summary>
    public static StillSettings Apply(
        StillSettings settings,
        string? metadataPath = null,
        string? exif = null,
        MetadataFormat? metadataFormat = null)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (!string.IsNullOrWhiteSpace(metadataPath))
        {
            settings.Metadata = metadataPath;
        }

        if (!string.IsNullOrWhiteSpace(exif))
        {
            settings.Exif = exif;
        }

        if (metadataFormat is not null)
        {
            settings.MetadataFormat = metadataFormat;
        }

        return settings;
    }

    /// <summary>
    /// Enable JSON metadata sidecar next to the output file.
    /// </summary>
    public static string SuggestMetadataPath(string outputPath, string extension = ".json")
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException("Output path is required.", nameof(outputPath));
        }

        return Path.ChangeExtension(outputPath, extension);
    }

    /// <summary>
    /// Enable pts sidecar next to the output video file.
    /// </summary>
    public static string SuggestPtsPath(string outputPath) => SuggestMetadataPath(outputPath, ".pts");
}
