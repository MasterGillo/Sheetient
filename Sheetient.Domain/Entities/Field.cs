using Sheetient.Domain.Interfaces;

namespace Sheetient.Domain.Entities
{
    public abstract class Field : IEntity
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        public required Page Page { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
