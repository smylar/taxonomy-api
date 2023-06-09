using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Taxonomy.Domain.Converter;

public class IntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //again some empty values as strings
        try
        {
            return reader.GetInt32();
        }
        catch (Exception) { }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

