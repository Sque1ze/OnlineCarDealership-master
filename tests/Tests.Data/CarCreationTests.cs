using System;
using Domain.Cars;
using Xunit;

namespace Tests.Data;

public class CarCreationTests
{
    [Fact]
    public void CarId_New_ShouldCreateUniqueNonEmptyIds()
    {
        // Act
        var id1 = CarId.New();
        var id2 = CarId.New();

        // Assert
        Assert.NotEqual(default, id1);
        Assert.NotEqual(Guid.Empty, id1.Value);

        Assert.NotEqual(default, id2);
        Assert.NotEqual(Guid.Empty, id2.Value);

        Assert.NotEqual(id1, id2);
        Assert.NotEqual(id1.Value, id2.Value);
    }
}