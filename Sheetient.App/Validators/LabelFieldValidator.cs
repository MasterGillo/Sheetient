using FluentValidation;
using Sheetient.App.Dtos.Sheet;

namespace Sheetient.App.Validators
{
    public class LabelFieldValidator : AbstractValidator<LabelFieldDto>
    {
        public LabelFieldValidator()
        {
            Include(new FieldValidator());
            RuleFor(x => x.Text).NotEmpty();
        }
    }
}
