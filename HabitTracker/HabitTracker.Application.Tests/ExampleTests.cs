using FluentAssertions;
using Xunit;

namespace HabitTracker.Application.Tests
{
    public class ExampleTests
    {
        [Fact]
        public void Should_Add_TwoNumbers_Correctly()
        {
            int result = 2 + 3;
            result.Should().Be(5);
        }
    }
}