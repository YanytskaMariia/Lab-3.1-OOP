using System.Collections.Generic;
using System.Linq;
using LinkCollector.Models;

namespace LinkCollector.Services
{
    /// <summary>
    /// Статичний клас-репозиторій для керування даними в пам'яті.
    /// Виконує роль бази даних для цієї лабораторної.
    /// </summary>
    public static class LinkRepository
    {
        // Головний список посилань
        private static List<ResourceLink> _links = new List<ResourceLink>();

        // Список доступних категорій
        private static List<string> _categories = new List<string> { "Навчання", "Робота", "Хобі" };

        /// <summary>
        /// Статичний конструктор для ініціалізації тестовими даними.
        /// </summary>
        static LinkRepository()
        {
            _links.Add(new ResourceLink
            {
                Title = "Clean Code",
                Author = "Robert C. Martin",
                Year = 2008,
                Type = LinkType.Book,
                Category = "Навчання",
                UrlOrSource = "Prentice Hall"
            });

            _links.Add(new ResourceLink
            {
                Title = "Документація Microsoft .NET",
                Author = "Microsoft",
                Year = 2023,
                Type = LinkType.WebResource,
                Category = "Робота",
                UrlOrSource = "https://learn.microsoft.com"
            });
        }

        /// <summary>
        /// Отримати всі посилання.
        /// </summary>
        public static List<ResourceLink> GetAll() => _links;

        /// <summary>
        /// Пошук посилань за текстом (назва або автор).
        /// </summary>
        public static List<ResourceLink> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return _links;

            return _links.Where(l =>
                l.Title.ToLower().Contains(query.ToLower()) ||
                l.Author.ToLower().Contains(query.ToLower())
            ).ToList();
        }

        // Методи додавання та видалення посилань
        public static void Add(ResourceLink link) => _links.Add(link);
        public static void Remove(ResourceLink link) => _links.Remove(link);

        // Методи роботи з категоріями
        public static List<string> GetCategories() => _categories;

        public static void AddCategory(string cat)
        {
            if (!_categories.Contains(cat)) _categories.Add(cat);
        }

        public static void RemoveCategory(string cat) => _categories.Remove(cat);
    }
}