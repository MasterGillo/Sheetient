using FluentValidation;
using Sheetient.App.Dtos.Sheet;

namespace Sheetient.App.Validators
{
    public class FieldValidator : AbstractValidator<FieldDto>
    {
        public FieldValidator()
        {
            RuleFor(x => x.FieldType).NotNull();
            RuleFor(x => x.PageId).NotNull();
            RuleFor(x => x.X).NotNull();
            RuleFor(x => x.Y).NotNull();
            RuleFor(x => x.Width).GreaterThan(0);
            RuleFor(x => x.Height).GreaterThan(0);
        }
    }
}
