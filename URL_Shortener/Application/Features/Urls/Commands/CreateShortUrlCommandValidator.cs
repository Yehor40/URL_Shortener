using FluentValidation;

namespace URL_Shortener.Application.Features.Urls.Commands;

public class CreateShortUrlCommandValidator : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlCommandValidator()
    {
        RuleFor(v => v.OriginalUrl)
            .NotEmpty().WithMessage("Original URL is required.")
            .MaximumLength(2048).WithMessage("Original URL must not exceed 2048 characters.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Invalid URL format. Please provide a full URL including protocol (e.g., https://).");

        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
