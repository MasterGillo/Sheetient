using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sheetient.App.Dtos.Sheet;
using Sheetient.Domain.Enums;

namespace Sheetient.App.Converters
{
    public class FieldConverter : JsonConverter<FieldDto>
    {
        public override void WriteJson(JsonWriter writer, FieldDto? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;

        public override FieldDto? ReadJson(JsonReader reader, Type objectType, FieldDto? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            FieldDto? result = null;

            if (jObject.TryGetValue("fieldType", out JToken? fieldTypeToken))
            {
                var fieldType = Enum.Parse<FieldType>(fieldTypeToken.ToString());
                switch (fieldType)
                {
                    case FieldType.Label:
                    {
                        result = new LabelFieldDto();

                        break;
                    }
                    case FieldType.TextInput:
                    {
                        result = new TextInputFieldDto();
                        break;
                    }
                }
            }

            if (result != null)
            {
                serializer.Populate(jObject.CreateReader(), result);
            }
            return result;

        }
    }
}
