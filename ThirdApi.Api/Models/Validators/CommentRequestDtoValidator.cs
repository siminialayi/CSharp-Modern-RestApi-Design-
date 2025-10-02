using Blog.Api.Models.DataTransferObjets.RequestDtos;
using FluentValidation;
using FluentValidation.Results;

namespace Blog.Api.Models.Validators;

/// <summary>
/// Defines the validation rules for the Comment creation/update request DTO.
/// </summary>
/// <remarks>
/// RESPONSIBILITY:
/// - Encapsulates input rule enforcement separate from transport (DTO) and domain (Entity) layers.
/// DESIGN PRINCIPLES:
/// - Single Responsibility: DTO remains a simple data container; validator handles rule logic.
/// - Fail Fast: Provides early feedback during model binding to reduce deeper pipeline failures.
/// RULE CATEGORIES:
/// - Structural: Ensures required identifiers are present (PostId).
/// - Content Quality: Enforces minimum and maximum length boundaries for Content.
/// EXTENSION GUIDELINES:
/// - Add profanity filtering or semantic checks via Custom validators if business mandates.
/// - For cross-field validation (e.g., conditional logic), use RuleFor(x => x.Property).Custom(...).
/// </remarks>
public class CommentRequestDtoValidator : AbstractValidator<CommentRequestDto>
    {
    public CommentRequestDtoValidator()
        {
        // Foreign key presence & integrity (syntactic level)
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("PostId is required and cannot be an empty GUID.");

        // Content integrity and bounded length to prevent abuse / storage inefficiency
        RuleFor(x => x.Content)
            .NotEmpty()
            .Must(name => !string.IsNullOrWhiteSpace(name?.Trim()))
            .WithMessage("Comment content is required.")
            .MinimumLength(5)
            .WithMessage("Comment content too short.")
            .MaximumLength(500)
            .WithMessage("Comment limit exceeded.");
        }
    }