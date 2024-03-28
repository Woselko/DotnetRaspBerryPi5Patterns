using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Encodings;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Helpers;

/// <summary>
/// StillSettings JsonConverter Helper.
/// </summary>
public class StillSettingsConverter<TStillSettings> : JsonConverter<TStillSettings> where TStillSettings : StillSettings
{
    /// <summary>
    /// Validate if can convert.
    /// </summary>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(StillSettings).IsAssignableFrom(typeToConvert);
    }

    /// <summary>
    /// Read from json.
    /// </summary>
    public override TStillSettings? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Type type = typeToConvert;

        reader.Read();

        if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == nameof(StillSettings.Encoding))
        {
            reader.Read();

            string? Encoding = reader.GetString() ?? string.Empty;

            if (Encoding == Settings.Enumerations.Encoding.Bmp.Value)
            {
                type = typeof(Bmp);
            }

            if (Encoding == Settings.Enumerations.Encoding.Jpg.Value)
            {
                type = typeof(Jpg);
            }

            if (Encoding == Settings.Enumerations.Encoding.Png.Value)
            {
                type = typeof(Png);
            }

            if (Encoding == Settings.Enumerations.Encoding.Rgb.Value)
            {
                type = typeof(Rgb);
            }

            if (Encoding == Settings.Enumerations.Encoding.Yuv420.Value)
            {
                type = typeof(Yuv420);
            }
        }
        else
        {
            return null;
        }

        TStillSettings? settings = (TStillSettings?)Activator.CreateInstance(type);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString() ?? string.Empty;

                reader.Read();

                PropertyInfo? property = type.GetProperty(propertyName);

                if (property != null && property.CanWrite)
                {
                    object? value = JsonSerializer.Deserialize(ref reader, property.PropertyType, options);

                    if (value != null)
                    {
                        property.SetValue(settings, value);
                    }
                }
            }
        }

        return settings;
    }

    /// <summary>
    /// Write to json.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, TStillSettings value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        Type type = value.GetType();

        PropertyInfo Encoding = type.GetProperty(nameof(StillSettings.Encoding))!;
        object? EncodingValue = Encoding.GetValue(value);

        if (EncodingValue != null)
        {
            writer.WritePropertyName(Encoding.Name);
            JsonSerializer.Serialize(writer, EncodingValue, options);
        }

        foreach (PropertyInfo property in type.GetProperties())
        {
            if (property.CanRead && property.Name != nameof(StillSettings.Encoding))
            {
                object? propertyValue = property.GetValue(value);
                if (propertyValue != null)
                {
                    writer.WritePropertyName(property.Name);
                    JsonSerializer.Serialize(writer, propertyValue, options);
                }
            }
        }
        writer.WriteEndObject();
    }
}