using Xunit;
using ApiWeb.Helpers; 

namespace ApiWeb.Tests.Helpers
{
    public class DateHelperTests
    {
        [Fact]
        public void ShouldReturnCorrectDateDifference()
        {
            // Arrange
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 1, 10);

            // Act
            var difference = DateHelper.CalculateDateDifference(startDate, endDate); // Corrigido

            // Assert
            Assert.Equal(9, difference);
        }
    }
}
