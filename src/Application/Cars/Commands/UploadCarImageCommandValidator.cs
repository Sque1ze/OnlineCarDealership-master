using FluentValidation;

namespace Application.Cars.Commands;

public class UploadCarImagesCommandValidator : AbstractValidator<UploadCarImagesCommand>
{
    public UploadCarImagesCommandValidator()
    {
        RuleFor(x => x.CarId).NotEmpty();
        RuleFor(x => x.Images).NotEmpty();
        RuleFor(x => x.Images).NotEmpty().WithMessage("No files provided");
        RuleForEach(x => x.Images).ChildRules(image =>
        {
            image.RuleFor(x => x.OriginalName).NotEmpty();
            image.RuleFor(x => x.FileStream).NotNull();
        });
    }
}