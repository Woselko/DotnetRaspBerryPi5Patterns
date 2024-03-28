using RaspCameraLibrary.Helpers;
using RaspCameraLibrary.Settings.Enumerations;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Encodings;

/// <summary>
/// Bmp Still Settings.
/// </summary>
[JsonConverter(typeof(StillSettingsConverter<Bmp>))]
public class Bmp : StillSettings
{
    /// <summary>
    /// Will use the BMP encoding.
    /// </summary>
    public override Encoding Encoding => Encoding.Bmp;
}