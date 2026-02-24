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
        /// <summary>
        /// Перевіряє пошук за запитом, який не збігається з жодним записом.
        /// Очікується отримання порожнього списку.
        /// </summary>
        [Fact]
        public void Search_NonExistentQuery_ReturnsEmptyList()
        {
            // Act
            var result = LinkRepository.Search("NonExistentAuthorOrTitle123");

            // Assert
            Assert.Empty(result);
        }

        /// <summary>
        /// Перевіряє, що пошук коректно обробляє пробіли в запиті (Trim не реалізований в коді, 
        /// тому це перевірка поточної поведінки або підстава для покращення).
        /// </summary>
        [Fact]
        public void Search_WhitespaceQuery_ReturnsAllLinks()
        {
            // Act
            var result = LinkRepository.Search("   ");

            // Assert
            Assert.Equal(LinkRepository.GetAll().Count, result.Count);
        }

        /// <summary>
        /// Перевіряє логіку валідації року видання. 
        /// Рік не може бути більшим за поточний (наприклад, 2026 для поточного моменту).
        /// Примітка: Цей тест допоможе виявити відсутність валідації в методі Add.
        /// </summary>
        [Fact]
        public void Add_FutureYear_ShouldThrowExceptionOrNotAdd()
        {
            // Arrange
            var futureLink = new ResourceLink
            {
                Title = "Future Book",
                Author = "Time Traveler",
                Year = System.DateTime.Now.Year + 1
            };

            // Act & Assert
            // Якщо у вас ще немає логіки валідації, цей тест вкаже на проблему.
            // Можна очікувати ArgumentException, якщо ви додасте перевірку в метод Add.
            Assert.Throws<System.ArgumentException>(() => LinkRepository.Add(futureLink));
        }

        /// <summary>
        /// Перевіряє видалення категорії, яка не існує в списку.
        /// Очікується, що метод завершиться без помилок (Safe remove).
        /// </summary>
        [Fact]
        public void RemoveCategory_NonExistent_DoesNotThrow()
        {
            // Arrange
            string fakeCat = "NonExistentCategory";

            // Act & Assert (якщо метод не кидає виключення, тест пройдено)
            var exception = Record.Exception(() => LinkRepository.RemoveCategory(fakeCat));
            Assert.Null(exception);
        }

        /// <summary>
        /// Перевіряє додавання посилання з порожніми обов'язковими полями.
        /// </summary>
        [Theory]
        [InlineData(null, "Author")]
        [InlineData("Title", null)]
        [InlineData("", "")]
        public void Add_InvalidData_ShouldHandleErrors(string title, string author)
        {
            // Arrange
            var invalidLink = new ResourceLink { Title = title, Author = author, Year = 2020 };

            // Act & Assert
            // Перевіряємо, чи система захищена від додавання "битої" моделі
            Assert.Throws<System.ArgumentException>(() => LinkRepository.Add(invalidLink));
        }
       
    }
}