using Domain.Cars;

namespace Api.Dtos;

public record CarDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<CategoryCarDto>? Categories,
    IReadOnlyList<CarImageDto>? Images)
{
    public static CarDto FromDomainModel(Car car)
        => new(
            car.Id.Value,
            car.Name,
            car.Description,
            car.Price,
            car.StockQuantity,
            car.CreatedAt,
            car.UpdatedAt,
            car.Categories != null
                ? car.Categories.Select(CategoryCarDto.FromDomainModel).ToList()
                : [],
            car.Images != null
                ? car.Images.Select(CarImageDto.FromDomainModel).ToList()
                : []);
}

public record CarImageDto(
    Guid Id,
    string OriginalName,
    string FilePath)
{
    public static CarImageDto FromDomainModel(CarImage image)
        => new(
            image.Id.Value,
            image.OriginalName,
            image.GetFilePath());
}

public record CreateCarDto(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    IReadOnlyList<Guid> Categories);

public record UpdateCarDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    IReadOnlyList<Guid> Categories);