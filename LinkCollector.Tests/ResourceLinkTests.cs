using Xunit;
using LinkCollector.Models;
using System;

namespace LinkCollector.Tests
{
    /// <summary>
    /// Набір тестів для перевірки коректності роботи моделі ResourceLink.
    /// </summary>
    public class ResourceLinkTests
    {
        /// <summary>
        /// Перевіряє, чи конструктор генерує унікальний ідентифікатор (Guid) для кожного нового об'єкта.
        /// </summary>
        [Fact]
        public void Constructor_GeneratesUniqueId()
        {
            // Arrange & Act
            var link1 = new ResourceLink();
            var link2 = new ResourceLink();

            // Assert
            Assert.NotEqual(Guid.Empty, link1.Id); // ID не має бути пустим
            Assert.NotEqual(link1.Id, link2.Id);   // ID двох різних об'єктів мають відрізнятися
        }

        /// <summary>
        /// Перевіряє, чи метод ToString() повертає рядок у форматі "Назва (Автор) - Рік".
        /// </summary>
        [Fact]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var link = new ResourceLink
            {
                Title = "Test Book",
                Author = "Test Author",
                Year = 2024
            };

            // Act
            var result = link.ToString();

            // Assert
            Assert.Equal("Test Book (Test Author) - 2024", result);
        }
    }
}