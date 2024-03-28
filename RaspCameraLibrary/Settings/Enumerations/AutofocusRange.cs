using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Specifies the autofocus range
/// </summary>
[JsonConverter(typeof(EnumerationConverter<AutofocusRange, string>))]
public class AutofocusRange(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// Focuses from reasonably close to infinity
    /// </summary>
    public static AutofocusRange Normal => new("normal");

    /// <summary>
    /// Focuses only on close objects, including the closest focal distances supported by the camera
    /// </summary>
    public static AutofocusRange Macro => new("macro");

    /// <summary>
    /// Will focus on the entire range, from the very closest objects to infinity.
    /// </summary>
    public static AutofocusRange Full => new("full");
}