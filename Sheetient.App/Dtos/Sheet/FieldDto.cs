using Newtonsoft.Json;
using Sheetient.App.Converters;
using Sheetient.Domain.Enums;

namespace Sheetient.App.Dtos.Sheet
{
    [JsonConverter(typeof(FieldConverter))]
    public abstract class FieldDto
    {
        public int Id { get; set; }
        public abstract FieldType FieldType { get; set; }
        public int PageId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
