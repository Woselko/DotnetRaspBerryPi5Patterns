using RaspCameraLibrary.Helpers;
using RaspCameraLibrary.Settings.Enumerations;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Encodings;

/// <summary>
/// Jpg Still Settings.
/// </summary>
[JsonConverter(typeof(StillSettingsConverter<Jpg>))]
public class Jpg : StillSettings
{
    /// <summary>
    /// Will use the JPEG encoding.
    /// </summary>
    public override Encoding Encoding => Encoding.Jpg;

    /// <summary>
    /// Set the JPEG quality parameter
    /// </summary>
    [Argument("--quality")]
    public uint? Quality
    {
        get => _quality;
        set => _quality = value is null ? null : uint.Clamp(value.Value, 0, 100);
    }
    private uint? _quality = null;
}