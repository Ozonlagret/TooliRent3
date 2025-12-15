using Application.DTOs.Requests;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators
{
    public class ToolFilterRequestValidator : AbstractValidator<ToolFilterRequest>
    {
        public ToolFilterRequestValidator()
        {
            RuleFor(x => x.ToolId)
                .GreaterThan(0).When(x => x.ToolId.HasValue)
                .WithMessage("Tool ID must be greater than 0.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).When(x => x.CategoryId.HasValue)
                .WithMessage("Category ID must be greater than 0.");

            RuleFor(x => x.Status)
                .Must(BeValidToolStatus).When(x => !string.IsNullOrEmpty(x.Status))
                .WithMessage("Invalid tool status. Valid values are: Operational, UnderMaintenance, Retired.");

            RuleFor(x => x.Availability)
                .Must(BeValidAvailability).When(x => !string.IsNullOrEmpty(x.Availability))
                .WithMessage("Invalid availability. Valid values are: Available, Rented, Reserved.");
        }

        private bool BeValidToolStatus(string? status)
        {
            if (string.IsNullOrEmpty(status))
                return true;
            return Enum.TryParse<ToolStatus>(status, true, out _);
        }

        private bool BeValidAvailability(string? availability)
        {
            if (string.IsNullOrEmpty(availability))
                return true;
            return Enum.TryParse<ToolAvailability>(availability, true, out _);
        }
    }
}
