using Domain.Cars;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface ICarImageRepository
{
    Task<CarImage> AddAsync(CarImage entity, CancellationToken cancellationToken);
    Task<IReadOnlyList<CarImage>> AddRangeAsync(IReadOnlyList<CarImage> entities, CancellationToken cancellationToken);
    Task<Option<CarImage>> GetByIdAsync(CarImageId id, CancellationToken cancellationToken);
    Task<CarImage> DeleteAsync(CarImage entity, CancellationToken cancellationToken);
}