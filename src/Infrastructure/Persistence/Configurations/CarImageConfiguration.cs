using Domain.Cars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CarImageConfiguration : IEntityTypeConfiguration<CarImage>
{
    public void Configure(EntityTypeBuilder<CarImage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new CarImageId(x));

        builder.Property(x => x.OriginalName).IsRequired().HasColumnType("varchar(255)");

        builder.Property(x => x.CarId).HasConversion(x => x.Value, x => new CarId(x));
        builder.HasOne<Car>()
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.CarId)
            .HasConstraintName("fk_car_images_cars_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}