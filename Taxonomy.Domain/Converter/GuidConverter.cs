using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Taxonomy.Domain.Converter;

public class GuidConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Guid conversion is built in, but source has empty strings for some of these which the default does not deal with
        try
        {
            Guid guid;
            if (Guid.TryParse(reader.GetString(), out guid)) {
                return guid;
            }
        }
        catch (Exception) { }
        return Guid.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

