using System;
using System.Collections.Generic;
using System.Linq;
using LinkCollector.Models;

namespace LinkCollector.Services
{
    /// <summary>
    /// Репозиторій для керування даними в пам'яті.
    /// Тепер — не статичний клас, реалізує <see cref="ILinkRepository"/>.
    /// Початкові дані ініціалізуються в конструкторі.
    /// </summary>
    public class LinkRepository : ILinkRepository
    {
        // Головний список посилань
        private readonly List<ResourceLink> _links;

        // Список доступних категорій
        private readonly List<string> _categories;

        /// <summary>
        /// Ініціалізує новий екземпляр репозиторію та додає тестові дані.
        /// </summary>
        public LinkRepository()
        {
            _links = new List<ResourceLink>();
            _categories = new List<string> { "Навчання", "Робота", "Хобі" };

            // Початкові дані
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
        public List<ResourceLink> GetAll() => _links;

        /// <summary>
        /// Пошук посилань за текстом (назва або автор).
        /// </summary>
        public List<ResourceLink> Search(string query)
        {
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
        public void Add(ResourceLink link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            if (string.IsNullOrWhiteSpace(link.Title) || string.IsNullOrWhiteSpace(link.Author))
            {
                throw new ArgumentException("Назва та автор є обов'язковими для заповнення.");
            }

            if (link.Year > DateTime.Now.Year)
            {
                throw new ArgumentException($"Рік видання не може бути більшим за поточний ({DateTime.Now.Year}).");
            }

            _links.Add(link);
        }

        /// <summary>
        /// Видаляє посилання зі списку.
        /// </summary>
        public void Remove(ResourceLink link)
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
        public List<string> GetCategories() => _categories;

        /// <summary>
        /// Додає нову категорію, якщо такої ще немає.
        /// </summary>
        public void AddCategory(string cat)
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
        public void RemoveCategory(string cat)
        {
            if (!string.IsNullOrEmpty(cat))
            {
                _categories.Remove(cat);
            }
        }
    }
}