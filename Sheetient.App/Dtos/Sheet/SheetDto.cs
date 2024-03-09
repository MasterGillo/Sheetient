using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Sheetient.App.Mapping;

namespace Sheetient.App.Dtos.Sheet
{
    public class SheetDto : IMapping<Domain.Entities.Sheet>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<PageDto> Pages { get; set; } = [];

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SheetDto, Domain.Entities.Sheet>().ReverseMap();
            profile.CreateMap<JsonPatchDocument<SheetDto>, JsonPatchDocument<Domain.Entities.Sheet>>();
            profile.CreateMap<Operation<SheetDto>, Operation<Domain.Entities.Sheet>>();
        }
    }
}
