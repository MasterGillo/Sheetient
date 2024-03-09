using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Sheetient.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected void ThrowValidationException()
        {
            var errors = ModelState.Where(x => x.Value != null).SelectMany(x => x.Value!.Errors.Select(y => y.ErrorMessage));
            var message = errors.Any() ? string.Join(" ", errors) : "Validation failed.";
            throw new ValidationException(message);
        }
    }
}
