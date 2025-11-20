using FluentValidation;
using RealEstatePlatform.Application.DTOs.Listing;

namespace RealEstatePlatform.Application.Validators;

/// <summary>
/// Validator for CreateListingDto
/// </summary>
public class CreateListingDtoValidator : AbstractValidator<CreateListingDto>
{
    public CreateListingDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Area)
            .GreaterThan(0).WithMessage("Area must be greater than 0");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");

        RuleFor(x => x.WardId)
            .NotEmpty().WithMessage("Ward is required");

        RuleFor(x => x.ContactName)
            .NotEmpty().WithMessage("Contact name is required")
            .MaximumLength(200).WithMessage("Contact name cannot exceed 200 characters");

        RuleFor(x => x.ContactPhone)
            .NotEmpty().WithMessage("Contact phone is required")
            .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Invalid phone number format");

        RuleFor(x => x.ContactEmail)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail))
            .WithMessage("Invalid email address");

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo(0).When(x => x.Bedrooms.HasValue)
            .WithMessage("Bedrooms cannot be negative");

        RuleFor(x => x.Bathrooms)
            .GreaterThanOrEqualTo(0).When(x => x.Bathrooms.HasValue)
            .WithMessage("Bathrooms cannot be negative");

        RuleFor(x => x.YearBuilt)
            .GreaterThan(1800).When(x => x.YearBuilt.HasValue)
            .LessThanOrEqualTo(DateTime.Now.Year).When(x => x.YearBuilt.HasValue)
            .WithMessage($"Year built must be between 1800 and {DateTime.Now.Year}");
    }
}
