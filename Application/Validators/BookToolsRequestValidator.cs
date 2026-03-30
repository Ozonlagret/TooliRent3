using Application.DTOs.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;

namespace Application.Validators
{
    public class BookToolsRequestValidator : AbstractValidator<BookToolsRequest>
    {
        public BookToolsRequestValidator()
        {
            RuleFor(x => x.ToolIds)
                .NotEmpty().WithMessage("At least one tool must be selected for booking.")
                .Must(ids => ids.All(id => id > 0)).WithMessage("All tool IDs must be valid.")
                .Must(IDataSerializer => IDataSerializer.Distinct().Count() == IDataSerializer.Length).WithMessage("Duplicate tool IDs are not allowed.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

            RuleFor(x => x)
                .Must(x => (x.EndDate.Date - x.StartDate.Date).TotalDays >= 1)
                .WithMessage("Booking must span at least 1 full day.");

            RuleFor(x => x)
                .Must(x => (x.EndDate - x.StartDate).TotalDays <= 30)
                .WithMessage("Booking duration cannot exceed 30 days.");
        }
    }
}
