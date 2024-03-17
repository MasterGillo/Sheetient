using Sheetient.App.Mapping;
using Sheetient.Domain.Entities;

namespace Sheetient.App.Dtos.Sheet
{
    public record PageDto : IMapping<Page>
    {
        public int Id { get; init; }
        public int SheetId { get; init; }
        public required string Name { get; init; }
        public required string Colour { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public bool ShowGrid { get; init; }
        public required string GridColour { get; init; }
        public int GridSpacingX { get; init; }
        public int GridSpacingY { get; init; }
        public int Order { get; init; }

        public List<FieldDto> Fields { get; init; } = [];
    }
}
