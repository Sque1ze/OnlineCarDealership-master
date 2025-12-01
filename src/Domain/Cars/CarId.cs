namespace Domain.Cars;

public record CarId(Guid Value)
{
    public static CarId Empty() => new(Guid.Empty);
    public static CarId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}