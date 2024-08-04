using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Telerik_ForumTeamProject.Helpers;

namespace Telerik_ForumTeamProject.Tests.Helpers
{
    [TestClass]
    public class DateTimeFormatterTests
    {
        [TestMethod]
        public void FormatToStandard_ShouldReturnCorrectFormat_WhenGivenValidDateTime()
        {
            // Arrange
            DateTime dateTime = new DateTime(2023, 7, 30, 14, 30, 0);
            string expectedFormat = "30.07.2023 14:30";

            // Act
            string result = DateTimeFormatter.FormatToStandard(dateTime);

            // Assert
            Assert.AreEqual(expectedFormat, result);
        }

        [TestMethod]
        [DataRow(2024, 12, 25, 9, 45, 0, "25.12.2024 09:45")]
        [DataRow(2020, 1, 1, 0, 0, 0, "01.01.2020 00:00")]
        [DataRow(1999, 11, 30, 23, 59, 59, "30.11.1999 23:59")]
        public void FormatToStandard_ShouldReturnCorrectFormat_ForVariousDateTimes(int year, int month, int day, int hour, int minute, int second, string expectedFormat)
        {
            // Arrange
            DateTime dateTime = new DateTime(year, month, day, hour, minute, second);

            // Act
            string result = DateTimeFormatter.FormatToStandard(dateTime);

            // Assert
            Assert.AreEqual(expectedFormat, result);
        }

        [TestMethod]
        public void FormatToStandard_ShouldReturnCorrectFormat_WhenDateTimeHasMilliseconds()
        {
            // Arrange
            DateTime dateTime = new DateTime(2022, 10, 20, 12, 15, 30, 500);
            string expectedFormat = "20.10.2022 12:15";

            // Act
            string result = DateTimeFormatter.FormatToStandard(dateTime);

            // Assert
            Assert.AreEqual(expectedFormat, result);
        }
    }
}
