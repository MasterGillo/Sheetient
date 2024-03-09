using Sheetient.Domain.Interfaces;

namespace Sheetient.Domain.Entities
{
    public class Page : IEntity
    {
        public int Id { get; set; }
        public int SheetId { get; set; }
        public required Sheet Sheet { get; set; }
        public required string Name { get; set; }
        public required string Colour { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool ShowGrid { get; set; }
        public required string GridColour { get; set; }
        public int GridSpacingX { get; set; }
        public int GridSpacingY { get; set; }
        public int Order { get; set; }

        public List<Field> Fields { get; set; } = [];
    }
}
