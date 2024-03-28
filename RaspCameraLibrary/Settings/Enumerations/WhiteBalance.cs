using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Sets the white balance mode
/// </summary>
[JsonConverter(typeof(EnumerationConverter<WhiteBalance, string>))]
public class WhiteBalance(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// Automatic white balance 2500K to 8000K
    /// </summary>
    public static WhiteBalance Auto => new("auto");

    /// <summary>
    /// Incandescent white balance 2500K to 3000K
    /// </summary>
    public static WhiteBalance Incandescent => new("incandescent");

    /// <summary>
    /// Tungsten white balance 3000K to 3500K
    /// </summary>
    public static WhiteBalance Tungsten => new("tungsten");

    /// <summary>
    /// Fluorescent white balance 4000K to 4700K
    /// </summary>
    public static WhiteBalance Fluorescent => new("fluorescent");

    /// <summary>
    /// Indoor white balance 3000K to 5000K
    /// </summary>
    public static WhiteBalance Indoor => new("indoor");

    /// <summary>
    /// Daylight white balance 5500K to 6500K
    /// </summary>
    public static WhiteBalance Daylight => new("daylight");

    /// <summary>
    /// Cloudy white balance 7000K to 8500K
    /// </summary>
    public static WhiteBalance Cloudy => new("cloudy");

    /// <summary>
    /// A custom range would have to be defined in the camera tuning file
    /// </summary>
    public static WhiteBalance Custom => new("custom");
}