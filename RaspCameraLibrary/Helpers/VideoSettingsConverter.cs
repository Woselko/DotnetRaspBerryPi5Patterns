using RaspCameraLibrary.Settings;
using RaspCameraLibrary.Settings.Codecs;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaspCameraLibrary.Helpers;

/// <summary>
/// VideoSettings JsonConverter Helper.
/// </summary>
public class VideoSettingsConverter<TVideoSettings> : JsonConverter<TVideoSettings> where TVideoSettings : VideoSettings
{
    /// <summary>
    /// Validate if can convert.
    /// </summary>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(VideoSettings).IsAssignableFrom(typeToConvert);
    }

    /// <summary>
    /// Read from json.
    /// </summary>
    public override TVideoSettings? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Type type = typeToConvert;

        reader.Read();

        if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == nameof(VideoSettings.Codec))
        {
            reader.Read();

            string? Codec = reader.GetString() ?? string.Empty;

            if (Codec == Settings.Enumerations.Codec.H264.Value)
            {
                type = typeof(H264);
            }

            if (Codec == Settings.Enumerations.Codec.Mjpeg.Value)
            {
                type = typeof(Mjpeg);
            }

            if (Codec == Settings.Enumerations.Codec.Yuv420.Value)
            {
                type = typeof(Yuv420);
            }
        }
        else
        {
            return null;
        }

        TVideoSettings? settings = (TVideoSettings?)Activator.CreateInstance(type);

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
    public override void Write(Utf8JsonWriter writer, TVideoSettings value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        Type type = value.GetType();

        PropertyInfo Codec = type.GetProperty(nameof(VideoSettings.Codec))!;
        object? CodecValue = Codec.GetValue(value);

        if (CodecValue != null)
        {
            writer.WritePropertyName(Codec.Name);
            JsonSerializer.Serialize(writer, CodecValue, options);
        }

        foreach (PropertyInfo property in type.GetProperties())
        {
            if (property.CanRead && property.Name != nameof(VideoSettings.Codec))
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