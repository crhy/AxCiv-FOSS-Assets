using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhyCiv.Engine.SaveLoad.SerializationUtils;

/// <summary>
/// Reads a string dictionary written either as a JSON object or as the array of
/// <c>{ "Key": ..., "Value": ... }</c> pairs older builds produced.
/// <para>
/// The save writer treated every dictionary as a plain sequence, so it came out as
/// an array of key/value objects while the reader expected an object. Any save
/// holding a unit with script data on it -- a barbarian's horde flag, for one --
/// therefore could not be read back at all. The writer produces an object now, and
/// this accepts both, so a game saved before the fix still loads.
/// </para>
/// </summary>
public class ForgivingDictionaryConverter : JsonConverter<Dictionary<string, string>>
{
    public override Dictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var result = new Dictionary<string, string>();

        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return result;

            case JsonTokenType.StartObject:
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    var key = reader.GetString() ?? string.Empty;
                    reader.Read();
                    result[key] = ReadScalar(ref reader);
                }

                return result;

            case JsonTokenType.StartArray:
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    ReadLegacyPair(ref reader, result);
                }

                return result;

            default:
                throw new JsonException(
                    $"Expected an object or an array for a dictionary, found {reader.TokenType}.");
        }
    }

    /// <summary>One <c>{ "Key": ..., "Value": ... }</c> entry from an older save.</summary>
    private static void ReadLegacyPair(ref Utf8JsonReader reader, IDictionary<string, string> into)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            return;
        }

        string? key = null;
        var value = string.Empty;
        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            var name = reader.GetString();
            reader.Read();
            if (string.Equals(name, "Key", StringComparison.OrdinalIgnoreCase))
            {
                key = ReadScalar(ref reader);
            }
            else if (string.Equals(name, "Value", StringComparison.OrdinalIgnoreCase))
            {
                value = ReadScalar(ref reader);
            }
            else
            {
                reader.Skip();
            }
        }

        if (key != null)
        {
            into[key] = value;
        }
    }

    /// <summary>
    /// Script data is stored as strings, but an older writer emitted whatever type
    /// the value happened to be, so numbers and booleans have to be accepted too.
    /// </summary>
    private static string ReadScalar(ref Utf8JsonReader reader) => reader.TokenType switch
    {
        JsonTokenType.String => reader.GetString() ?? string.Empty,
        JsonTokenType.Number => reader.GetDouble().ToString(System.Globalization.CultureInfo.InvariantCulture),
        JsonTokenType.True => "1",
        JsonTokenType.False => "0",
        JsonTokenType.Null => string.Empty,
        _ => string.Empty
    };

    public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var (key, entry) in value)
        {
            writer.WriteString(key, entry);
        }

        writer.WriteEndObject();
    }
}
