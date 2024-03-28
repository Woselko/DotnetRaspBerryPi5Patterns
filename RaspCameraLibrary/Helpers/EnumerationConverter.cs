using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Helpers;

/// <summary>
/// Enumeration JsonConverter Helper.
/// </summary>
public class EnumerationConverter<TEnumeration, TValue> : JsonConverter<TEnumeration>
    where TEnumeration : Enumeration<TValue>
    where TValue : IComparable, IComparable<TValue>
{

    /// <summary>
    /// Read from json.
    /// </summary>
    public override TEnumeration? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            return null;
        }

        string? Value = reader.GetString();

        if (Value is null)
        {
            return null;
        }

        return Enumeration<TValue>.Get<TEnumeration>(Value);
    }

    /// <summary>
    /// Write to json.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, TEnumeration value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}