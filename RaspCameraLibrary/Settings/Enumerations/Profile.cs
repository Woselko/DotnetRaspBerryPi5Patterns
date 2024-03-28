using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// H264 Profile
/// </summary>
[JsonConverter(typeof(EnumerationConverter<Profile, string>))]
public class Profile(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// Baseline profile
    /// </summary>
    public static Profile Baseline => new("baseline");

    /// <summary>
    /// Main profile
    /// </summary>
    public static Profile Main => new("main");

    /// <summary>
    /// High profile
    /// </summary>
    public static Profile High => new("high");
}