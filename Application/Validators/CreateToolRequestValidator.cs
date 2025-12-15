using Application.DTOs.Requests;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators
{
    public class CreateToolRequestValidator : AbstractValidator<CreateToolRequest>
    {
        public CreateToolRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tool name is required.")
                .MaximumLength(200).WithMessage("Tool name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.RentalPricePerDay)
                .GreaterThan(0).WithMessage("Rental price must be greater than 0.")
                .LessThanOrEqualTo(10000).WithMessage("Rental price must not exceed 10,000.");

            RuleFor(x => x.Condition)
                .NotEmpty().WithMessage("Condition is required.")
                .MaximumLength(50).WithMessage("Condition must not exceed 50 characters.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Valid category ID is required.");

            RuleFor(x => x.ToolStatus)
                .NotEmpty().WithMessage("Tool status is required.")
                .Must(BeValidToolStatus).WithMessage("Invalid tool status. Valid values are: Operational, UnderMaintenance, Retired.");

            RuleFor(x => x.Availability)
                .NotEmpty().WithMessage("Availability is required.")
                .Must(BeValidAvailability).WithMessage("Invalid availability. Valid values are: Available, Rented, Reserved.");
        }

        private bool BeValidToolStatus(string status)
        {
            return Enum.TryParse<ToolStatus>(status, true, out _);
        }

        private bool BeValidAvailability(string availability)
        {
            return Enum.TryParse<ToolAvailability>(availability, true, out _);
        }
    }
}
