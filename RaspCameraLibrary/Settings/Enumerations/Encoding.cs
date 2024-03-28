using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Encoding
/// </summary>
[JsonConverter(typeof(EnumerationConverter<Encoding, string>))]
public class Encoding(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// Will use the Jpg encoding.
    /// </summary>
    public static Encoding Jpg => new("jpg");

    /// <summary>
    /// Will use the Png encoding.
    /// </summary>
    public static Encoding Png => new("png");

    /// <summary>
    /// Will use the Rgb encoding.
    /// </summary>
    public static Encoding Rgb => new("rgb");

    /// <summary>
    /// Will use the Bmp encoding.
    /// </summary>
    public static Encoding Bmp => new("bmp");

    /// <summary>
    /// Will use the Yuv420 encoding.
    /// </summary>
    public static Encoding Yuv420 => new("yuv420");
}