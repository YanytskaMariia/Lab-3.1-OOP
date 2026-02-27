using System;
using System.Collections.Generic;
using System.Text;
using LinkCollector.Models;

namespace LinkCollector.Services
{
    /// <summary>
    /// Сервіс для генерації текстового представлення посилань у різних форматах.
    /// </summary>
    public class CitationService : ICitationService
    {
        /// <summary>
        /// Генерує рядок одного посилання відповідно до стилю.
        /// </summary>
        public string GenerateCitation(ResourceLink link, CitationStyle style)
        {
            if (link == null) return string.Empty;

            switch (style)
            {
                case CitationStyle.DSTU_8302:
                    // Приклад: Іванов І.І. Назва книги. — Видавництво, 2020.
                    return $"{link.Author}. {link.Title}. — {link.UrlOrSource}, {link.Year}.";

                case CitationStyle.Harvard:
                    // Приклад: Ivanov, I. (2020) 'Title'. Available at: Source.
                    return $"{link.Author} ({link.Year}) '{link.Title}'. Available at: {link.UrlOrSource}.";

                case CitationStyle.BibTeX:
                    // Формат для LaTeX з використанням системного розділювача рядків
                    var nl = Environment.NewLine;
                    return $"@misc{{ link_{link.GetHashCode()},{nl}" +
                           $"  author = \"{link.Author}\",{nl}" +
                           $"  title = \"{link.Title}\",{nl}" +
                           $"  year = \"{link.Year}\",{nl}" +
                           $"  howpublished = \"{link.UrlOrSource}\"{nl}" +
                           $"}}";

                default:
                    return link.ToString();
            }
        }

        /// <summary>
        /// Генерує повний список посилань для експорту.
        /// </summary>
        public string GenerateList(List<ResourceLink> links, CitationStyle style)
        {
            if (links == null || links.Count == 0) return string.Empty;

            var sb = new StringBuilder();
            foreach (var link in links)
            {
                sb.AppendLine(GenerateCitation(link, style));

                // Додатковий відступ між записами BibTeX для читабельності
                if (style == CitationStyle.BibTeX)
                {
                    sb.AppendLine();
                }
            }
            return sb.ToString().TrimEnd(); // Видаляємо зайві хвости в кінці всього списку
        }
    }
}