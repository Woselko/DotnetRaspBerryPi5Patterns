using RaspCameraLibrary.Helpers;
using RaspCameraLibrary.Settings.Enumerations;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Codecs;

/// <summary>
/// Mjpeg Video Settings.
/// </summary>
[JsonConverter(typeof(VideoSettingsConverter<Mjpeg>))]
public class Mjpeg : VideoSettings
{
    /// <summary>
    /// Will use the MJPEG codec.
    /// </summary>
    public override Codec Codec => Codec.Mjpeg;

    /// <summary>
    /// Set the MJPEG quality parameter (mjpeg only)
    /// </summary>
    [Argument("--quality")]
    public uint? Quality
    {
        get => _quality;
        set => _quality = value is null ? null : uint.Clamp(value.Value, 0, 100);
    }
    private uint? _quality = null;
}