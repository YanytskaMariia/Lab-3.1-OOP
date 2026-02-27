using LinkCollector.Models;
using LinkCollector.Tests;



namespace LinkCollector.Tests
{
    /// <summary>
    /// Набір тестів для перевірки коректності роботи моделі ResourceLink.
    /// </summary>
    public class ResourceLinkTests
    {
        [Fact]
        public void Constructor_GeneratesUniqueId()
        {
            var link1 = new ResourceLink();
            var link2 = new ResourceLink();

            Assert.NotEqual(Guid.Empty, link1.Id);
            Assert.NotEqual(link1.Id, link2.Id);
        }

        [Fact]
        public void ToString_ReturnsCorrectFormat()
        {
            var year = DateTime.Now.Year;
            var link = new ResourceLink
            {
                Title = "Test Book",
                Author = "Test Author",
                Year = year
            };

            var expected = $"{link.Title} ({link.Author}) - {link.Year}";
            Assert.Equal(expected, link.ToString());
        }

        [Fact]
        public void Properties_StoreAndReturnCorrectValues()
        {
            var link = new ResourceLink();
            var expectedTitle = "C# in Depth";
            var expectedAuthor = "Jon Skeet";
            var expectedUrl = "https://manning.com";
            var expectedYear = 2019;
            var expectedCategory = "Programming";
            var expectedType = LinkType.Book;

            link.Title = expectedTitle;
            link.Author = expectedAuthor;
            link.UrlOrSource = expectedUrl;
            link.Year = expectedYear;
            link.Category = expectedCategory;
            link.Type = expectedType;

            Assert.Equal(expectedTitle, link.Title);
            Assert.Equal(expectedAuthor, link.Author);
            Assert.Equal(expectedUrl, link.UrlOrSource);
            Assert.Equal(expectedYear, link.Year);
            Assert.Equal(expectedCategory, link.Category);
            Assert.Equal(expectedType, link.Type);
        }

        [Fact]
        public void ToString_WithNullValues_DoesNotThrow_AndMatchesExpected()
        {
            var link = new ResourceLink
            {
                Title = null,
                Author = null,
                Year = 0
            };

            var exception = Record.Exception(() => link.ToString());
            Assert.Null(exception);

            var expected = $"{link.Title} ({link.Author}) - {link.Year}";
            Assert.Equal(expected, link.ToString());
        }

        [Fact]
        public void ToString_WithEmptyValues_DoesNotThrow_AndMatchesExpected()
        {
            var year = DateTime.Now.Year;
            var link = new ResourceLink
            {
                Title = string.Empty,
                Author = string.Empty,
                Year = year
            };

            var exception = Record.Exception(() => link.ToString());
            Assert.Null(exception);

            var expected = $"{link.Title} ({link.Author}) - {link.Year}";
            Assert.Equal(expected, link.ToString());
        }

        [Fact]
        public void Id_RemainsConstant_AfterPropertyUpdate()
        {
            var link = new ResourceLink();
            var initialId = link.Id;

            link.Title = "Updated Title";
            link.Year = DateTime.Now.Year;

            Assert.Equal(initialId, link.Id);
        }

        [Fact]
        public void DefaultPropertyValues_AreReasonable()
        {
            var link = new ResourceLink();

            Assert.NotEqual(Guid.Empty, link.Id);
            Assert.Null(link.Title);
            Assert.Null(link.Author);
            Assert.Null(link.UrlOrSource);
            Assert.Equal(0, link.Year); // default(int) == 0
            Assert.Null(link.Category);
            // default enum value is 0 => first enum entry (Book) — assert type is within defined enum
            Assert.IsType<LinkType>(link.Type);
        }
    }
}