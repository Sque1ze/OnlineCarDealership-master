using Domain.Cars;

namespace Application.Common.Interfaces.Queries;

public interface ICarQueries
{
    Task<IReadOnlyList<Car>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Car>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken);
}