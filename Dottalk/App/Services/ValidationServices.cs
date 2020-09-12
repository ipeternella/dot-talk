using Dottalk.App.DTOs;
using FluentValidation;

//
// Summary:
//   Validators used by the application based on Fluent Validation.
namespace Dottalk.App.Services
{
    public class ChatRoomCreationValidator : AbstractValidator<ChatRoomCreationRequestDTO>
    {
        public ChatRoomCreationValidator()
        {
            RuleFor(dto => dto.Name)
                .NotEmpty()
                .MaximumLength(100)
                .Matches("[a-zA-Z]")
                .WithMessage("Room name must contain only letters and be up to 100 chars.");

            RuleFor(dto => dto.ActiveConnectionsLimit)
                .InclusiveBetween(2, 10)
                .WithMessage("Maximum active user connections must be a value between 2 and 10 concurrent users.");
        }
    }

    public class UserCreationValidator : AbstractValidator<UserCreationRequestDTO>
    {
        public UserCreationValidator()
        {
            RuleFor(dto => dto.Name)
                .NotEmpty()
                .MaximumLength(100)
                .Matches("[a-zA-Z]")
                .WithMessage("User name must contain only letters and be up to 100 chars.");
        }
    }
}