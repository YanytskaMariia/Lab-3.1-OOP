using System;
using Xunit;
using LinkCollector.Services;
using LinkCollector.Models;

namespace LinkCollector.Tests
{
    /// <summary>
    /// Тести для репозиторію LinkRepository (екземпляр на кожен тест).
    /// Перевіряють додавання, видалення та пошук даних.
    /// </summary>
    public class LinkRepositoryTests
    {
        private readonly LinkRepository _repo;

        public LinkRepositoryTests()
        {
            // Новий екземпляр репозиторію для кожного тесту (із початковими даними)
            _repo = new LinkRepository();
        }

        [Fact]
        public void GetAll_ReturnsInitialData()
        {
            var links = _repo.GetAll();

            Assert.NotNull(links);
            Assert.True(links.Count >= 2);
            Assert.Contains(links, l => l.Title == "Clean Code");
        }

        [Fact]
        public void Add_AddsNewLink()
        {
            var newLink = new ResourceLink
            {
                Title = "New Test Link",
                Author = "Tester",
                Year = DateTime.Now.Year
            };
            int countBefore = _repo.GetAll().Count;

            _repo.Add(newLink);

            var allLinks = _repo.GetAll();
            Assert.Equal(countBefore + 1, allLinks.Count);
            Assert.Contains(newLink, allLinks);

            // Cleanup
            _repo.Remove(newLink);
        }

        [Fact]
        public void Remove_RemovesExistingLink()
        {
            var link = new ResourceLink { Title = "To Remove", Author = "T", Year = DateTime.Now.Year };
            _repo.Add(link);

            _repo.Remove(link);

            Assert.DoesNotContain(link, _repo.GetAll());
        }

        [Fact]
        public void Search_FindsCorrectLinks_CaseInsensitive()
        {
            var resultTitle = _repo.Search("microsoft");
            var resultAuthor = _repo.Search("Robert");

            Assert.NotEmpty(resultTitle);
            Assert.Equal("Microsoft", resultTitle[0].Author);

            Assert.NotEmpty(resultAuthor);
            Assert.Contains(resultAuthor, l => l.Author == "Robert C. Martin");
        }

        [Fact]
        public void Search_EmptyString_ReturnsAll()
        {
            var result = _repo.Search("");

            Assert.Equal(_repo.GetAll().Count, result.Count);
        }

        [Fact]
        public void AddCategory_NewCategory_Added()
        {
            string newCat = "Scientific";

            _repo.AddCategory(newCat);

            Assert.Contains(newCat, _repo.GetCategories());

            _repo.RemoveCategory(newCat);
        }

        [Fact]
        public void AddCategory_Duplicate_NotAdded()
        {
            string cat = "Hobby";
            _repo.AddCategory(cat);
            int countBefore = _repo.GetCategories().Count;

            _repo.AddCategory(cat);

            int countAfter = _repo.GetCategories().Count;
            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public void Search_NonExistentQuery_ReturnsEmptyList()
        {
            var result = _repo.Search("NonExistentAuthorOrTitle123");

            Assert.Empty(result);
        }

        [Fact]
        public void Search_WhitespaceQuery_ReturnsAllLinks()
        {
            var result = _repo.Search("   ");

            Assert.Equal(_repo.GetAll().Count, result.Count);
        }

        [Fact]
        public void Add_FutureYear_ShouldThrowExceptionOrNotAdd()
        {
            var futureLink = new ResourceLink
            {
                Title = "Future Book",
                Author = "Time Traveler",
                Year = DateTime.Now.Year + 1
            };

            Assert.Throws<ArgumentException>(() => _repo.Add(futureLink));
        }

        [Fact]
        public void RemoveCategory_NonExistent_DoesNotThrow()
        {
            string fakeCat = "NonExistentCategory";

            var exception = Record.Exception(() => _repo.RemoveCategory(fakeCat));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(null, "Author")]
        [InlineData("Title", null)]
        [InlineData("", "")]
        public void Add_InvalidData_ShouldHandleErrors(string title, string author)
        {
            var invalidLink = new ResourceLink { Title = title, Author = author, Year = 2020 };

            Assert.Throws<ArgumentException>(() => _repo.Add(invalidLink));
        }
    }
}