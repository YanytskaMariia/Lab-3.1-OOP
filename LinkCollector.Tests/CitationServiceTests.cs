using LinkCollector.Models;
using LinkCollector.Services;
using LinkCollector.Tests;



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

        /// <summary>
        /// Перевіряє поведінку за замовчуванням (Default), коли передано невідомий стиль.
        /// </summary>
        [Fact]
        public void GenerateCitation_UnknownStyle_ReturnsToString()
        {
            // Arrange
            var link = new ResourceLink { Author = "U", Title = "T", Year = 2024 };
            var unknownStyle = (CitationStyle)999;

            // Act
            var result = _service.GenerateCitation(link, unknownStyle);

            // Assert
            Assert.Equal(link.ToString(), result);
        }

        /// <summary>
        /// Перевіряє, чи метод GenerateList коректно обробляє порожній список.
        /// </summary>
        [Fact]
        public void GenerateList_EmptyList_ReturnsEmptyString()
        {
            // Act
            var result = _service.GenerateList(new List<ResourceLink>(), CitationStyle.Harvard);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        /// <summary>
        /// Перевіряє специфічне форматування BibTeX у списку (подвійний розрив рядка).
        /// </summary>
        [Fact]
        public void GenerateList_BibTeX_AddsExtraNewLine()
        {
            // Arrange
            var links = new List<ResourceLink>
            {
                new ResourceLink { Author = "Auth1", Title = "Title1", Year = 2021, UrlOrSource = "Src1" },
                new ResourceLink { Author = "Auth2", Title = "Title2", Year = 2022, UrlOrSource = "Src2" }
            };

            // Act
            var result = _service.GenerateList(links, CitationStyle.BibTeX);

            // Assert
            string doubleLineBreak = Environment.NewLine + Environment.NewLine;
            Assert.Contains("}" + doubleLineBreak, result);
        }

        /// <summary>
        /// Перевіряє використання хеш-коду як ключа в BibTeX.
        /// </summary>
        [Fact]
        public void GenerateCitation_BibTeX_UsesHashCodeAsKey()
        {
            // Arrange
            var link = new ResourceLink { Author = "X", Title = "Y", Year = 2000 };
            string expectedKey = $"link_{link.GetHashCode()}";

            // Act
            var result = _service.GenerateCitation(link, CitationStyle.BibTeX);

            // Assert
            Assert.Contains(expectedKey, result);
        }

        /// <summary>
        /// Перевіряє, що метод повертає порожній рядок, якщо передано null замість об'єкта посилання.
        /// </summary>
        [Fact]
        public void GenerateCitation_NullLink_ReturnsEmptyString()
        {
            // Act
            var result = _service.GenerateCitation(null, CitationStyle.Harvard);

            // Assert
            Assert.Equal(string.Empty, result);
        }
    }
}