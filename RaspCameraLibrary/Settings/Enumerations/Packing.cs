using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Sets the packing mode of the camera
/// </summary>
[JsonConverter(typeof(EnumerationConverter<Packing, string>))]
public class Packing(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// Packed mode
    /// </summary>
    public static Packing Packed => new("P");

    /// <summary>
    /// Unpacked mode
    /// </summary>
    public static Packing Unpacked => new("U");
}