using Domain.Categories;
using Domain.Cars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CategoryCarsConfiguration : IEntityTypeConfiguration<CategoryCar>
{
    public void Configure(EntityTypeBuilder<CategoryCar> builder)
    {
        builder.HasKey(cf => new { cf.CategoryId, cf.CarId });

        builder.Property(x => x.CategoryId).HasConversion(x => x.Value, x => new CategoryId(x));
        builder.HasOne(x => x.Category)
            .WithMany(x => x.Cars)
            .HasForeignKey(x => x.CategoryId)
            .HasConstraintName("fk_category_cars_categories_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CarId).HasConversion(x => x.Value, x => new CarId(x));
        builder.HasOne(x => x.Car)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.CarId)
            .HasConstraintName("fk_category_cars_cars_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}