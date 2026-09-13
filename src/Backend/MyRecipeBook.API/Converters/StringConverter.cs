using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MyRecipeBook.API.Converters
{
    // CLasse reponsavel por converter as strings das requisições JSON, sempre antes do Controller e do Validator
    public partial class StringConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Esse value significa estamos lendo do JSON o que recebemos lá do Swagger, postman
            // Lembrando que esse método Trim, apenas remove os espaços do início e do fim da uma string.
            var value = reader.GetString()?.Trim();

            // value tem a possibilidade de ser null, se for já retornamos ele aqui.
            if (value == null)
                return null;
            
           return RemoveExtraWhiteSpaces().Replace(value, " ");
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

        // Definindo no Regex oq devemos procurar, ou seja, mais de um espaço aglomerado.
        [GeneratedRegex(@"\s+")]
        private static partial Regex RemoveExtraWhiteSpaces();
    }
}
