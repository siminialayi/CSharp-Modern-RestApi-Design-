using Blog.Api.Models.DataTransferObjets.RequestDtos;
using FluentValidation;
using FluentValidation.Results;

namespace Blog.Api.Models.Validators;

/// <summary>
/// Defines the validation rules for the Comment creation/update request DTO.
/// This separates validation logic from the DTO, adhering to the SRP.
/// </summary>
public class CommentRequestDtoValidator : AbstractValidator<CommentRequestDto>
    {
    public CommentRequestDtoValidator()
        {
        // Professional: Ensuring the Guid is not empty, which is a common validation point for foreign keys.
        RuleFor(x => x.PostId)
            .NotEmpty()
            .WithMessage("PostId is required and cannot be an empty GUID.");

        // Professional: Requiring Content and enforcing a minimum/maximum length for business rules.
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



    