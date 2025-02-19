using Sheetient.App.Mapping;

namespace Sheetient.App.Dtos.Sheet
{
    public record SheetSummaryDto : IMapping<Domain.Entities.Sheet>
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
    }
}
