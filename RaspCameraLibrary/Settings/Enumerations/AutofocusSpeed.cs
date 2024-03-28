using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Specifies the autofocus range
/// </summary>
[JsonConverter(typeof(EnumerationConverter<AutofocusSpeed, string>))]
public class AutofocusSpeed(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// The lens position will change at the normal speed
    /// </summary>
    public static AutofocusSpeed Normal => new("normal");

    /// <summary>
    /// The lens position may change more quickly
    /// </summary>
    public static AutofocusSpeed Fast => new("fast");
}