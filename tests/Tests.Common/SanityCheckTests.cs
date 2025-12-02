using Xunit;

namespace Tests.Common;

public class SanityCheckTests
{
    [Fact]
    public void True_ShouldBeTrue()
    {
        Assert.True(true);
    }

    [Fact]
    public void TwoPlusTwo_ShouldEqualFour()
    {
        Assert.Equal(4, 2 + 2);
    }
}
