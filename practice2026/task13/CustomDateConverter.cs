using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public class CustomDateConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? datetime = reader.GetString();
        if (string.IsNullOrEmpty(datetime))
        {
            throw new JsonException("Date value cannot be null or empty.");
        }

        if (!DateTime.TryParseExact(datetime, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
        {
            throw new JsonException($"Invalid date format, expected: {DateFormat}");
        }

        return date;
    }

    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
    {
        writer.WriteStringValue(date.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
