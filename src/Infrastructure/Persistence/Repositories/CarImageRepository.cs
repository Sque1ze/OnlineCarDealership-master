using Application.Common.Interfaces.Repositories;
using Domain.Cars;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CarImageRepository(ApplicationDbContext context) : ICarImageRepository
{
    public async Task<CarImage> AddAsync(CarImage entity, CancellationToken cancellationToken)
    {
        await context.CarImages.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IReadOnlyList<CarImage>> AddRangeAsync(
        IReadOnlyList<CarImage> entities,
        CancellationToken cancellationToken)
    {
        await context.CarImages.AddRangeAsync(entities, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<Option<CarImage>> GetByIdAsync(CarImageId id, CancellationToken cancellationToken)
    {
        var entity = await context.CarImages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity ?? Option<CarImage>.None;
    }

    public async Task<CarImage> DeleteAsync(CarImage entity, CancellationToken cancellationToken)
    {
        context.CarImages.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}