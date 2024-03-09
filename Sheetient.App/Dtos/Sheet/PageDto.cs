using Sheetient.App.Mapping;
using Sheetient.Domain.Entities;

namespace Sheetient.App.Dtos.Sheet
{
    public class PageDto : IMapping<Page>
    {
        public int Id { get; set; }
        public int SheetId { get; set; }
        public required string Name { get; set; }
        public required string Colour { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool ShowGrid { get; set; }
        public required string GridColour { get; set; }
        public int GridSpacingX { get; set; }
        public int GridSpacingY { get; set; }
        public int Order { get; set; }

        public List<FieldDto> Fields { get; set; } = [];
    }
}
