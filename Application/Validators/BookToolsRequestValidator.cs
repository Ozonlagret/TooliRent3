using Application.DTOs.Requests;
using FluentValidation;

namespace Application.Validators
{
    public class BookToolsRequestValidator : AbstractValidator<BookToolsRequest>
    {
        public BookToolsRequestValidator()
        {
            RuleFor(x => x.ToolIds)
                .NotEmpty().WithMessage("At least one tool must be selected for booking.")
                .Must(ids => ids.All(id => id > 0)).WithMessage("All tool IDs must be valid.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

            RuleFor(x => x)
                .Must(x => (x.EndDate - x.StartDate).TotalDays <= 30)
                .WithMessage("Booking duration cannot exceed 30 days.");
        }
    }
}
