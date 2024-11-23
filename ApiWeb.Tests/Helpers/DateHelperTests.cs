using Xunit;
using ApiWeb.Helpers;
using System;

namespace ApiWeb.Tests.Helpers
{
    public class DateHelperTests
    {
        [Theory]
        [InlineData("2023-01-01", "2023-01-10", 9)]
        [InlineData("2023-01-01", "2023-02-01", 31)]
        [InlineData("2023-01-01", "2023-01-01", 0)]
        public void CalculateDateDifference_DeveRetornarDiferencaCorreta(string startDateStr, string endDateStr, int expectedDays)
        {
            // Arrange
            var startDate = DateTime.Parse(startDateStr);
            var endDate = DateTime.Parse(endDateStr);

            // Act
            var difference = DateHelper.CalculateDateDifference(startDate, endDate);

            // Assert
            Assert.Equal(expectedDays, difference);
        }

        [Fact]
        public void IsValidTransactionDate_DataFutura_DeveRetornarFalso()
        {
            // Arrange
            var futureDate = DateTime.Now.AddDays(1);

            // Act
            var isValid = DateHelper.IsValidTransactionDate(futureDate);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValidTransactionDate_DataPassada_DeveRetornarTrue()
        {
            // Arrange
            var pastDate = DateTime.Now.AddDays(-1);

            // Act
            var isValid = DateHelper.IsValidTransactionDate(pastDate);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("2023-01-15", "2023-01-01")]
        [InlineData("2023-02-28", "2023-02-01")]
        public void GetFirstDayOfMonth_DeveRetornarPrimeiroDiaDoMes(string inputDate, string expectedDate)
        {
            // Arrange
            var date = DateTime.Parse(inputDate);
            var expected = DateTime.Parse(expectedDate);

            // Act
            var result = DateHelper.GetFirstDayOfMonth(date);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}