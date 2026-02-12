using System.Collections.Generic;
using System.Text;
using LinkCollector.Models;

namespace LinkCollector.Services
{
    /// <summary>
    /// Сервіс для генерації текстового представлення посилань у різних форматах.
    /// </summary>
    public class CitationService
    {
        /// <summary>
        /// Генерує рядок одного посилання відповідно до стилю.
        /// </summary>
        public string GenerateCitation(ResourceLink link, CitationStyle style)
        {
            switch (style)
            {
                case CitationStyle.DSTU_8302:
                    // Приклад: Іванов І.І. Назва книги. — Видавництво, 2020.
                    return $"{link.Author}. {link.Title}. — {link.UrlOrSource}, {link.Year}.";

                case CitationStyle.Harvard:
                    // Приклад: Ivanov, I. (2020) 'Title'. Available at: Source.
                    return $"{link.Author} ({link.Year}) '{link.Title}'. Available at: {link.UrlOrSource}.";

                case CitationStyle.BibTeX:
                    // Формат для LaTeX
                    return $"@misc{{ link_{link.GetHashCode()},\n" +
                           $"  author = \"{link.Author}\",\n" +
                           $"  title = \"{link.Title}\",\n" +
                           $"  year = \"{link.Year}\",\n" +
                           $"  howpublished = \"{link.UrlOrSource}\"\n" +
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
            var sb = new StringBuilder();
            foreach (var link in links)
            {
                sb.AppendLine(GenerateCitation(link, style));
                if (style == CitationStyle.BibTeX) sb.AppendLine(); // Відступ для BibTeX
            }
            return sb.ToString();
        }
    }
}