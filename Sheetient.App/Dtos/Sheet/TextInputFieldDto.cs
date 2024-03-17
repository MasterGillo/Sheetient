using AutoMapper;
using Sheetient.App.Mapping;
using Sheetient.Domain.Entities;
using Sheetient.Domain.Enums;

namespace Sheetient.App.Dtos.Sheet
{
    public record TextInputFieldDto : FieldDto, IMapping<TextInputField>
    {
        public override FieldType FieldType { get; init; } = FieldType.TextInput;
        public string LabelText { get; init; } = string.Empty;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<FieldDto, Field>()
                .Include<TextInputFieldDto, TextInputField>()
                .ReverseMap();
            profile.CreateMap<TextInputFieldDto, TextInputField>().ReverseMap();
        }
    }
}
