namespace Domain.Cars;

public record CarImageId(Guid Value)
{
    public static CarImageId Empty() => new(Guid.Empty);
    public static CarImageId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}