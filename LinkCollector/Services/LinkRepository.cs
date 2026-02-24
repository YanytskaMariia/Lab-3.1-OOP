using System;
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
            // Використовуємо пряме додавання в поле, щоб оминути валідацію конструктора для початкових даних,
            // або викликаємо Add, якщо дані гарантовано коректні.
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
            // Якщо запит порожній або складається лише з пробілів, повертаємо всі посилання
            if (string.IsNullOrWhiteSpace(query)) return _links;

            string lowerQuery = query.Trim().ToLower();

            return _links.Where(l =>
                (l.Title != null && l.Title.ToLower().Contains(lowerQuery)) ||
                (l.Author != null && l.Author.ToLower().Contains(lowerQuery))
            ).ToList();
        }

        /// <summary>
        /// Додає нове посилання з валідацією даних.
        /// </summary>
        /// <exception cref="ArgumentException">Виникає при некоректних даних або році з майбутнього.</exception>
        public static void Add(ResourceLink link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            // Валідація обов'язкових полів (для тесту Add_InvalidData_ShouldHandleErrors)
            if (string.IsNullOrWhiteSpace(link.Title) || string.IsNullOrWhiteSpace(link.Author))
            {
                throw new ArgumentException("Назва та автор є обов'язковими для заповнення.");
            }

            // Валідація року (для тесту Add_FutureYear_ShouldThrowExceptionOrNotAdd)
            if (link.Year > DateTime.Now.Year)
            {
                throw new ArgumentException($"Рік видання не може бути більшим за поточний ({DateTime.Now.Year}).");
            }

            _links.Add(link);
        }

        /// <summary>
        /// Видаляє посилання зі списку.
        /// </summary>
        public static void Remove(ResourceLink link)
        {
            if (link != null)
            {
                _links.Remove(link);
            }
        }

        // Методи роботи з категоріями

        /// <summary>
        /// Отримати список усіх категорій.
        /// </summary>
        public static List<string> GetCategories() => _categories;

        /// <summary>
        /// Додає нову категорію, якщо такої ще немає.
        /// </summary>
        public static void AddCategory(string cat)
        {
            if (string.IsNullOrWhiteSpace(cat)) return;

            if (!_categories.Contains(cat))
            {
                _categories.Add(cat);
            }
        }

        /// <summary>
        /// Видаляє категорію (безпечне видалення).
        /// </summary>
        public static void RemoveCategory(string cat)
        {
            if (!string.IsNullOrEmpty(cat))
            {
                _categories.Remove(cat);
            }
        }
    }
}