using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Sheetient.App.Mapping;

namespace Sheetient.App.Dtos.Sheet
{
    public record SheetDto : IMapping<Domain.Entities.Sheet>
    {
        public int? Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public List<PageDto> Pages { get; init; } = [];

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SheetDto, Domain.Entities.Sheet>().ReverseMap();
            profile.CreateMap<JsonPatchDocument<SheetDto>, JsonPatchDocument<Domain.Entities.Sheet>>();
            profile.CreateMap<Operation<SheetDto>, Operation<Domain.Entities.Sheet>>();
        }
    }
}
