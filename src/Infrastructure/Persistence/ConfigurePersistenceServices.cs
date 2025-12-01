using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class ConfigurePersistenceServices
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // 🔹 Ініціалізатор БД
        services.AddScoped<ApplicationDbContextInitialiser>();

        // ========== Cars ==========

        services.AddScoped<CarRepository>();
        services.AddScoped<ICarRepository>(provider => provider.GetRequiredService<CarRepository>());
        services.AddScoped<ICarQueries>(provider => provider.GetRequiredService<CarRepository>());

        // ========== Categories ==========

        services.AddScoped<CategoryRepository>();
        services.AddScoped<ICategoryRepository>(provider => provider.GetRequiredService<CategoryRepository>());
        // 🔥 ЦЬОГО РЯДКА НЕ ВИСТАЧАЛО
        services.AddScoped<ICategoryQueries>(provider => provider.GetRequiredService<CategoryRepository>());

        // Category–Car linking
        services.AddScoped<CategoryCarRepository>();
        services.AddScoped<ICategoryCarRepository>(provider => provider.GetRequiredService<CategoryCarRepository>());

        // ========== Car images ==========

        services.AddScoped<CarImageRepository>();
        services.AddScoped<ICarImageRepository>(provider => provider.GetRequiredService<CarImageRepository>());

        // ========== Customers ==========

        services.AddScoped<CustomerRepository>();
        services.AddScoped<ICustomerRepository>(provider => provider.GetRequiredService<CustomerRepository>());
        services.AddScoped<ICustomerQueries>(provider => provider.GetRequiredService<CustomerRepository>());

        // ========== Orders ==========

        services.AddScoped<OrderRepository>();
        services.AddScoped<IOrderRepository>(provider => provider.GetRequiredService<OrderRepository>());
        services.AddScoped<IOrderQueries>(provider => provider.GetRequiredService<OrderRepository>());
    }
}