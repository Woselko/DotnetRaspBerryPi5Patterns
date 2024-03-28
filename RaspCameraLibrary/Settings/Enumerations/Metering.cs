using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Sets the metering mode of the AEC/AGC algorithm
/// </summary>
[JsonConverter(typeof(EnumerationConverter<Metering, string>))]
public class Metering(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// Centre weighted metering (which is the default)
    /// </summary>
    public static Metering Centre => new("centre");

    /// <summary>
    /// Spot metering
    /// </summary>
    public static Metering Spot => new("spot");

    /// <summary>
    /// Average or whole frame metering
    /// </summary>
    public static Metering Average => new("average");

    /// <summary>
    /// Custom metering mode which would have to be defined in the camera tuning file.
    /// </summary>
    public static Metering Custom => new("custom");
}