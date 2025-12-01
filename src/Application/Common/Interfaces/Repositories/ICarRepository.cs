using Domain.Cars;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface ICarRepository
{
    Task<Car> AddAsync(Car entity, CancellationToken cancellationToken);
    Task<Car> UpdateAsync(Car entity, CancellationToken cancellationToken);
    Task<Car> DeleteAsync(Car entity, CancellationToken cancellationToken);
    Task<Option<Car>> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<Option<Car>> GetByIdAsync(CarId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Car>> GetByIdsAsync(IReadOnlyList<CarId> ids, CancellationToken cancellationToken);
}