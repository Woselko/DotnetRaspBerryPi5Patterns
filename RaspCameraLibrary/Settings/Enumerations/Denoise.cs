using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Denoise
/// </summary>
[JsonConverter(typeof(EnumerationConverter<Denoise, string>))]
public class Denoise(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// It always enables standard spatial denoise. It uses extra fast colour denoise for video, and high quality colour denoise for stills capture. Preview does not enable any extra colour denoise at all.
    /// </summary>
    public static Denoise Auto => new("auto");

    /// <summary>
    /// Disables spatial and colour denoise.
    /// </summary>
    public static Denoise Off => new("off");

    /// <summary>
    /// Disables colour denoise.
    /// </summary>
    public static Denoise CdnOff => new("cdn_off");

    /// <summary>
    /// Uses fast colour denoise.
    /// </summary>
    public static Denoise CdnFast => new("cdn_fast");

    /// <summary>
    /// Uses high quality colour denoise. Not appropriate for video/viewfinder due to reduced throughput.
    /// </summary>
    public static Denoise CdnHq => new("cdn_hq");
}