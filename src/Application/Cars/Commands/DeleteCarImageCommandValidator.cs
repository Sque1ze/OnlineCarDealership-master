using FluentValidation;

namespace Application.Cars.Commands;

public class DeleteCarImageCommandValidator : AbstractValidator<DeleteCarImageCommand>
{
    public DeleteCarImageCommandValidator()
    {
        RuleFor(x => x.CarId).NotEmpty();
        RuleFor(x => x.ImageId).NotEmpty();
    }
}