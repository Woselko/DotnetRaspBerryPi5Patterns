using RaspCameraLibrary.Helpers;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Settings.Enumerations;

/// <summary>
/// Format to save the metadata in
/// </summary>
[JsonConverter(typeof(EnumerationConverter<MetadataFormat, string>))]
public class MetadataFormat(string Value, string? Description = null) : Enumeration<string>(Value, Description)
{
    /// <summary>
    /// Will save the metadata in a text file.
    /// </summary>
    public static MetadataFormat Txt => new("txt");

    /// <summary>
    /// Will save the metadata in a json file.
    /// </summary>
    public static MetadataFormat Json => new("json");
}