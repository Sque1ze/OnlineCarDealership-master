using Domain.Categories;
using Domain.Cars;

namespace Application.Common.Interfaces.Repositories;

public interface ICategoryCarRepository
{
    Task<IReadOnlyList<CategoryCar>> AddRangeAsync(
        IReadOnlyList<CategoryCar> entities,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CategoryCar>> RemoveRangeAsync(
        IReadOnlyList<CategoryCar> entities,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CategoryCar>> GetByCarIdAsync(
        CarId carId,
        CancellationToken cancellationToken);
}