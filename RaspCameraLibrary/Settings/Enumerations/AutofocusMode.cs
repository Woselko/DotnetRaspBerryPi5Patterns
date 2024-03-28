using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Specifies the autofocus mode to use
/// </summary>
using RaspCameraLibrary.Helpers; // Add this line

[JsonConverter(typeof(RaspCameraLibrary.Helpers.EnumerationConverter<AutofocusMode, string>))]
public class AutofocusMode(string Value, string? Description = null) : RaspCameraLibrary.Helpers.Enumeration<string>(Value, Description)
{
    /// <summary>
    /// Do not move the lens at all, but it can be set with the --lens-position option
    /// </summary>
    public static AutofocusMode Manual => new("manual");

    /// <summary>
    /// Does not move the lens except for an autofocus sweep when the camera starts
    /// </summary>
    public static AutofocusMode Auto => new("auto");

    /// <summary>
    /// Adjusts the lens position automatically as the scene changes.
    /// </summary>
    public static AutofocusMode Continuous => new("continuous");
}