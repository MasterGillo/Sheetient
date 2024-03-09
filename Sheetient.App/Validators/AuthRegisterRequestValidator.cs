using FluentValidation;
using Sheetient.App.Dtos.Auth;

namespace Sheetient.App.Validators
{
    public class AuthRegisterRequestValidator : AbstractValidator<AuthRegisterRequestDto>
    {
        public AuthRegisterRequestValidator()
        {
            RuleFor(x => x.DisplayName)
                .NotEmpty()
                .Matches(@"^[\w-\._\@\+]+$").WithMessage("Display Name can only contain letters, numbers, or the special characters -._@+");
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}
