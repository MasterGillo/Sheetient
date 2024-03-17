using Newtonsoft.Json;
using Sheetient.App.Converters;
using Sheetient.Domain.Enums;

namespace Sheetient.App.Dtos.Sheet
{
    [JsonConverter(typeof(FieldConverter))]
    public abstract record FieldDto
    {
        public int Id { get; init; }
        public abstract FieldType FieldType { get; init; }
        public int PageId { get; init; }
        public int X { get; init; }
        public int Y { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
    }
}
