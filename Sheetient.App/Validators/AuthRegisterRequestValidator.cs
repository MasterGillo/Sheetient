using FluentValidation;
using Sheetient.App.Dtos.Auth;

namespace Sheetient.App.Validators
{
    public class AuthRegisterRequestValidator : AbstractValidator<AuthRegisterRequestDto>
    {
        public AuthRegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(6)
                .Matches(@"^[\w-\._\@\+]+$");
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}
