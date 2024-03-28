using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Bit Depth.
/// </summary>
[JsonConverter(typeof(EnumerationConverter<BitDepth, uint>))]
public class BitDepth(uint Value, string? Description = null) : Enumeration<uint>(Value, Description)
{
    /// <summary>
    /// 8 bit depth.
    /// </summary>
    public static BitDepth Bit10 => new(10);

    /// <summary>
    /// 12 bit depth.
    /// </summary>
    public static BitDepth Bit12 => new(12);
}