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
        /// <summary>
        /// Перевіряє коректність присвоєння та зчитування всіх властивостей моделі.
        /// Гарантує, що дані не спотворюються під час збереження в об'єкті.
        /// </summary>
        [Fact]
        public void Properties_StoreAndReturnCorrectValues()
        {
            // Arrange
            var link = new ResourceLink();
            var expectedTitle = "C# in Depth";
            var expectedAuthor = "Jon Skeet";
            var expectedUrl = "https://manning.com";
            var expectedYear = 2019;
            var expectedCategory = "Programming";
            var expectedType = LinkType.Book;

            // Act
            link.Title = expectedTitle;
            link.Author = expectedAuthor;
            link.UrlOrSource = expectedUrl;
            link.Year = expectedYear;
            link.Category = expectedCategory;
            link.Type = expectedType;

            // Assert
            Assert.Equal(expectedTitle, link.Title);
            Assert.Equal(expectedAuthor, link.Author);
            Assert.Equal(expectedUrl, link.UrlOrSource);
            Assert.Equal(expectedYear, link.Year);
            Assert.Equal(expectedCategory, link.Category);
            Assert.Equal(expectedType, link.Type);
        }

        /// <summary>
        /// Перевіряє поведінку методу ToString(), якщо текстові властивості є порожніми або null.
        /// Допомагає переконатися, що метод не викликає NullReferenceException.
        /// </summary>
        [Theory]
        [InlineData(null, null, 0)]
        [InlineData("", "", 2026)]
        public void ToString_HandlesEmptyData_DoesNotThrow(string title, string author, int year)
        {
            // Arrange
            var link = new ResourceLink
            {
                Title = title,
                Author = author,
                Year = year
            };

            // Act
            var exception = Record.Exception(() => link.ToString());

            // Assert
            Assert.Null(exception);
            Assert.Contains(year.ToString(), link.ToString());
        }

        /// <summary>
        /// Перевіряє, чи ідентифікатор Id залишається незмінним після встановлення інших властивостей.
        /// </summary>
        [Fact]
        public void Id_RemainsConstant_AfterPropertyUpdate()
        {
            // Arrange
            var link = new ResourceLink();
            Guid initialId = link.Id;

            // Act
            link.Title = "Updated Title";
            link.Year = 2025;

            // Assert
            Assert.Equal(initialId, link.Id);
        }
    }
}