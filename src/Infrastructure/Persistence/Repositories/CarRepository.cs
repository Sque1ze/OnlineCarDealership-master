using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Categories;
using Domain.Cars;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CarRepository(ApplicationDbContext context) : ICarRepository, ICarQueries
{
    
    public async Task<Option<Car>> GetByIdAsync(CarId id, CancellationToken cancellationToken)
    {
        var entity = await context.Cars
            .Include(x => x.Categories)!
            .ThenInclude(x => x.Category)
            .Include(x => x.Images)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

        return entity ?? Option<Car>.None;
    }
    
    public async Task<IReadOnlyList<Car>> GetByIdsAsync(IReadOnlyList<CarId> ids, CancellationToken cancellationToken)
    {
        return await context.Cars
            .Include(x => x.Categories)!
            .ThenInclude(x => x.Category)
            .Include(x => x.Images)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Car>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var entity = await context.Cars
            .Include(x => x.Images)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity ?? Option<Car>.None;
    }

    public async Task<Car> AddAsync(Car entity, CancellationToken cancellationToken)
    {
        await context.Cars.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Car> UpdateAsync(Car entity, CancellationToken cancellationToken)
    {
        context.Cars.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Car> DeleteAsync(Car entity, CancellationToken cancellationToken)
    {
        context.Cars.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IReadOnlyList<Car>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Cars
            .Include(x => x.Categories)!
            .ThenInclude(x => x.Category)
            .Include(x => x.Images)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    
public async Task<(IReadOnlyList<Car> Cars, int TotalCount)> SearchAsync(
    string? searchTerm,
    int page,
    int pageSize,
    CancellationToken cancellationToken)
{
    if (page <= 0) page = 1;
    if (pageSize <= 0 || pageSize > 100) pageSize = 10;

    var query = context.Cars
        .Include(x => x.Categories)!.ThenInclude(x => x.Category)
        .Include(x => x.Images)
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        var term = searchTerm.Trim().ToLower();
        query = query.Where(x =>
            x.Name.ToLower().Contains(term) ||
            x.Description.ToLower().Contains(term));
    }

    var totalCount = await query.CountAsync(cancellationToken);

    var cars = await query
        .OrderBy(x => x.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    return (cars, totalCount);
}

public async Task<IReadOnlyList<Car>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        var typedCategoryId = new CategoryId(categoryId);

        return await context.Cars
            .Include(x => x.Categories)!
            .ThenInclude(x => x.Category)
            .Include(x => x.Images)
            .Where(x => x.Categories!.Any(c => c.CategoryId == typedCategoryId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}