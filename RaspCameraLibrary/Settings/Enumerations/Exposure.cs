using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Changing the exposure profile should not affect the overall exposure of an image, but the sport mode will tend to prefer shorter exposure times and larger gains to achieve the same net result.
/// </summary>
[JsonConverter(typeof(EnumerationConverter<Exposure, string>))]
public class Exposure(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// Will use the normal exposure.
    /// </summary>
    public static Exposure Normal => new("normal");

    /// <summary>
    /// Will use the sport exposure.
    /// </summary>
    public static Exposure Sport => new("sport");

    /// <summary>
    /// Will use the night exposure.
    /// </summary>
    public static Exposure Long => new("long");
}