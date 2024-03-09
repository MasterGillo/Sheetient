using FluentValidation;
using Sheetient.App.Dtos.Sheet;

namespace Sheetient.App.Validators
{
    public class SheetValidator : AbstractValidator<SheetDto>
    {
        public SheetValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleForEach(x => x.Pages).SetValidator(new PageValidator());
        }
    }
}
