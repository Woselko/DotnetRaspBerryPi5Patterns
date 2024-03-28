using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// H264 Level
/// </summary>
[JsonConverter(typeof(EnumerationConverter<Level, string>))]
public class Level(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// Level 4
    /// </summary>
    public static Level Level4 => new("4");

    /// <summary>
    /// Level 4.1
    /// </summary>
    public static Level Level41 => new("4.1");

    /// <summary>
    /// Level 4.2
    /// </summary>
    public static Level Level42 => new("4.2");
}