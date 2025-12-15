using Application.DTOs.Requests;
using FluentValidation;

namespace Application.Validators
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.")
                .MinimumLength(20).WithMessage("Invalid refresh token format.");
        }
    }
}
