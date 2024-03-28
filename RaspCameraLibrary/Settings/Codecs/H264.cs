using RaspCameraLibrary.Helpers;
using RaspCameraLibrary.Settings.Enumerations;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Codecs;

/// <summary>
/// H264 Video Settings.
/// </summary>
[JsonConverter(typeof(VideoSettingsConverter<H264>))]
public class H264 : VideoSettings
{
    /// <summary>
    /// Will use the H264 codec.
    /// </summary>
    public override Codec Codec => Codec.H264;

    /// <summary>
    /// Set the video bitrate for encoding, in bits/second (h264 only)
    /// </summary>
    [Argument("--bitrate")]
    public uint? Bitrate { get; set; } = null;

    /// <summary>
    /// Set the encoding profile (h264 only)
    /// </summary>
    [Argument("--profile")]
    public Profile? Profile { get; set; } = null;

    /// <summary>
    /// Set the encoding level (h264 only)
    /// </summary>
    [Argument("--level")]
    public Level? Level { get; set; } = null;

    /// <summary>
    /// Set the intra frame period (h264 only)
    /// </summary>
    [Argument("--intra")]
    public uint? Intra { get; set; } = null;

    /// <summary>
    /// Force PPS/SPS header with every I frame (h264 only)
    /// </summary>
    [Argument("--inline")]
    public bool? Inline { get; set; } = null;
}