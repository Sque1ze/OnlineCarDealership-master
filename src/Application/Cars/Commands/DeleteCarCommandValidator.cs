using FluentValidation;

namespace Application.Cars.Commands;

public class DeleteCarCommandValidator : AbstractValidator<DeleteCarCommand>
{
    public DeleteCarCommandValidator()
    {
        RuleFor(x => x.CarId).NotEmpty();
    }
}