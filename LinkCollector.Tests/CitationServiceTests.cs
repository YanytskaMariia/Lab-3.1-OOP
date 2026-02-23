using Xunit;
using LinkCollector.Services;
using LinkCollector.Models;
using System.Collections.Generic;

namespace LinkCollector.Tests
{
    /// <summary>
    /// Тести для перевірки генерації бібліографічних описів у різних форматах.
    /// </summary>
    public class CitationServiceTests
    {
        private readonly CitationService _service;

        public CitationServiceTests()
        {
            _service = new CitationService();
        }

        /// <summary>
        /// Перевіряє генерацію посилання за стандартом ДСТУ 8302:2015.
        /// </summary>
        [Fact]
        public void GenerateCitation_DSTU_ReturnsCorrectString()
        {
            // Arrange
            var link = new ResourceLink
            {
                Author = "Author A.",
                Title = "My Book",
                Year = 2020,
                UrlOrSource = "Kyiv"
            };

            // Act
            var result = _service.GenerateCitation(link, CitationStyle.DSTU_8302);

            // Assert
            Assert.Equal("Author A.. My Book. — Kyiv, 2020.", result);
        }

        /// <summary>
        /// Перевіряє генерацію посилання у Гарвардському стилі.
        /// </summary>
        [Fact]
        public void GenerateCitation_Harvard_ReturnsCorrectString()
        {
            // Arrange
            var link = new ResourceLink
            {
                Author = "Smith, J.",
                Title = "Web Article",
                Year = 2021,
                UrlOrSource = "http://site.com"
            };

            // Act
            var result = _service.GenerateCitation(link, CitationStyle.Harvard);

            // Assert
            Assert.Equal("Smith, J. (2021) 'Web Article'. Available at: http://site.com.", result);
        }

        /// <summary>
        /// Перевіряє, чи формат BibTeX містить необхідні ключові поля (author, title, year).
        /// </summary>
        [Fact]
        public void GenerateCitation_BibTeX_ContainsKeywords()
        {
            // Arrange
            var link = new ResourceLink
            {
                Author = "Dev",
                Title = "Code",
                Year = 2022,
                UrlOrSource = "GitHub"
            };

            // Act
            var result = _service.GenerateCitation(link, CitationStyle.BibTeX);

            // Assert
            Assert.Contains("@misc{", result);
            Assert.Contains("author = \"Dev\"", result);
            Assert.Contains("title = \"Code\"", result);
            Assert.Contains("year = \"2022\"", result);
        }

        /// <summary>
        /// Перевіряє генерацію списку посилань: чи результат містить дані всіх переданих об'єктів.
        /// </summary>
        [Fact]
        public void GenerateList_ReturnsMultipleLines()
        {
            // Arrange
            var links = new List<ResourceLink>
            {
                new ResourceLink { Author = "A", Title = "T1", Year = 1, UrlOrSource = "S1" },
                new ResourceLink { Author = "B", Title = "T2", Year = 2, UrlOrSource = "S2" }
            };

            // Act
            var result = _service.GenerateList(links, CitationStyle.DSTU_8302);

            // Assert
            Assert.Contains("A. T1.", result);
            Assert.Contains("B. T2.", result);
        }
    }
}