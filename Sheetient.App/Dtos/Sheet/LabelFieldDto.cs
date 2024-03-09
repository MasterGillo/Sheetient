using AutoMapper;
using Sheetient.App.Mapping;
using Sheetient.Domain.Entities;
using Sheetient.Domain.Enums;

namespace Sheetient.App.Dtos.Sheet
{
    public class LabelFieldDto : FieldDto, IMapping<LabelField>
    {
        public override FieldType FieldType { get; set; } = FieldType.Label;
        public string Text { get; set; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<FieldDto, Field>()
                .Include<LabelFieldDto, LabelField>()
                .ReverseMap();
            profile.CreateMap<LabelFieldDto, LabelField>().ReverseMap();
        }
    }
}
