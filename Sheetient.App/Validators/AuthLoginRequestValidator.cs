using FluentValidation;
using Sheetient.App.Dtos.Auth;

namespace Sheetient.App.Validators
{
    public class AuthLoginRequestValidator : AbstractValidator<AuthLoginRequestDto>
    {
        public AuthLoginRequestValidator()
        {
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty();
            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
