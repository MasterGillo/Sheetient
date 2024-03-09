using FluentValidation;
using Sheetient.App.Dtos.Sheet;

namespace Sheetient.App.Validators
{
    public class PageValidator : AbstractValidator<PageDto>
    {
        public PageValidator()
        {
            RuleFor(x => x.SheetId).NotNull();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Colour).NotEmpty();
            RuleFor(x => x.Width).GreaterThan(0);
            RuleFor(x => x.Height).GreaterThan(0);
            RuleFor(x => x.GridColour).NotEmpty();
            RuleFor(x => x.GridSpacingX).GreaterThan(0);
            RuleFor(x => x.GridSpacingY).GreaterThan(0);
            RuleFor(x => x.Order).GreaterThan(0);
            RuleForEach(x => x.Fields).SetInheritanceValidator(x =>
            {
                x.Add(new LabelFieldValidator());
                x.Add(new TextInputFieldValidator());
            });
        }
    }
}
