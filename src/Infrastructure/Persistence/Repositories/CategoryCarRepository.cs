using Application.Common.Interfaces.Repositories;
using Domain.Categories;
using Domain.Cars;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CategoryCarRepository(ApplicationDbContext context) : ICategoryCarRepository
{
    public async Task<IReadOnlyList<CategoryCar>> AddRangeAsync(
        IReadOnlyList<CategoryCar> entities,
        CancellationToken cancellationToken)
    {
        await context.CategoryCars.AddRangeAsync(entities, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<IReadOnlyList<CategoryCar>> RemoveRangeAsync(
        IReadOnlyList<CategoryCar> entities,
        CancellationToken cancellationToken)
    {
        context.CategoryCars.RemoveRange(entities);
        await context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<IReadOnlyList<CategoryCar>> GetByCarIdAsync(
        CarId carId,
        CancellationToken cancellationToken)
    {
        return await context.CategoryCars
            .AsNoTracking()
            .Where(x => x.CarId.Equals(carId))
            .ToListAsync(cancellationToken);
    }
}