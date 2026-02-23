using Xunit;
using LinkCollector.Services;
using LinkCollector.Models;
using System.Linq;

namespace LinkCollector.Tests
{
    /// <summary>
    /// Тести для статичного репозиторію LinkRepository.
    /// Перевіряють додавання, видалення та пошук даних.
    /// </summary>
    [Collection("LinkRepositoryTests")] // Запобігає паралельному запуску тестів, оскільки репозиторій статичний
    public class LinkRepositoryTests
    {
        /// <summary>
        /// Перевіряє, що метод GetAll повертає початкові дані (ініціалізовані в конструкторі).
        /// </summary>
        [Fact]
        public void GetAll_ReturnsInitialData()
        {
            // Arrange & Act
            var links = LinkRepository.GetAll();

            // Assert
            Assert.NotNull(links);
            Assert.True(links.Count >= 2);
            Assert.Contains(links, l => l.Title == "Clean Code");
        }

        /// <summary>
        /// Перевіряє додавання нового посилання до репозиторію.
        /// </summary>
        [Fact]
        public void Add_AddsNewLink()
        {
            // Arrange
            var newLink = new ResourceLink
            {
                Title = "New Test Link",
                Author = "Tester",
                Year = 2024
            };
            int countBefore = LinkRepository.GetAll().Count;

            // Act
            LinkRepository.Add(newLink);

            // Assert
            var allLinks = LinkRepository.GetAll();
            Assert.Equal(countBefore + 1, allLinks.Count);
            Assert.Contains(newLink, allLinks);

            // Cleanup (Очищення після тесту)
            LinkRepository.Remove(newLink);
        }

        /// <summary>
        /// Перевіряє видалення існуючого посилання з репозиторію.
        /// </summary>
        [Fact]
        public void Remove_RemovesExistingLink()
        {
            // Arrange
            var link = new ResourceLink { Title = "To Remove", Author = "T" };
            LinkRepository.Add(link);

            // Act
            LinkRepository.Remove(link);

            // Assert
            Assert.DoesNotContain(link, LinkRepository.GetAll());
        }

        /// <summary>
        /// Перевіряє пошук посилань за частиною назви або автора (без урахування регістру).
        /// </summary>
        [Fact]
        public void Search_FindsCorrectLinks_CaseInsensitive()
        {
            // Arrange
            // Використовуємо дані, що вже є в репозиторії ("Microsoft")

            // Act
            var resultTitle = LinkRepository.Search("microsoft");
            var resultAuthor = LinkRepository.Search("Robert");

            // Assert
            Assert.NotEmpty(resultTitle);
            Assert.Equal("Microsoft", resultTitle[0].Author);

            Assert.NotEmpty(resultAuthor);
            Assert.Contains(resultAuthor, l => l.Author == "Robert C. Martin");
        }

        /// <summary>
        /// Перевіряє, що пошук по порожньому рядку повертає всі записи.
        /// </summary>
        [Fact]
        public void Search_EmptyString_ReturnsAll()
        {
            // Act
            var result = LinkRepository.Search("");

            // Assert
            Assert.Equal(LinkRepository.GetAll().Count, result.Count);
        }

        /// <summary>
        /// Перевіряє додавання нової унікальної категорії.
        /// </summary>
        [Fact]
        public void AddCategory_NewCategory_Added()
        {
            // Arrange
            string newCat = "Scientific";

            // Act
            LinkRepository.AddCategory(newCat);

            // Assert
            Assert.Contains(newCat, LinkRepository.GetCategories());

            // Cleanup
            LinkRepository.RemoveCategory(newCat);
        }

        /// <summary>
        /// Перевіряє, що дублікат категорії не додається повторно.
        /// </summary>
        [Fact]
        public void AddCategory_Duplicate_NotAdded()
        {
            // Arrange
            string cat = "Hobby";
            LinkRepository.AddCategory(cat); // Ця категорія вже може існувати або додається вперше
            int countBefore = LinkRepository.GetCategories().Count;

            // Act
            LinkRepository.AddCategory(cat); // Спроба додати дублікат

            // Assert
            int countAfter = LinkRepository.GetCategories().Count;
            Assert.Equal(countBefore, countAfter);
        }
    }
}