using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Codec
/// </summary>
[JsonConverter(typeof(EnumerationConverter<Codec, string>))]
public class Codec(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// Will use the H264 codec.
    /// </summary>
    public static Codec H264 => new("h264");

    /// <summary>
    /// Will use the MJPEG codec.
    /// </summary>
    public static Codec Mjpeg => new("mjpeg");

    /// <summary>
    /// Will use the YUV420 codec.
    /// </summary>
    public static Codec Yuv420 => new("yuv420");
}