using System;

namespace LinkCollector.Models
{
    /// <summary>
    /// Модель даних, що представляє одне посилання на ресурс.
    /// </summary>
    public class ResourceLink
    {
        /// <summary>
        /// Унікальний ідентифікатор запису.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; }       
        public string Author { get; set; }      
        public string UrlOrSource { get; set; } 
        public int Year { get; set; }           
        public string Category { get; set; }    
        public LinkType Type { get; set; }      

        /// <summary>
        /// Перевизначення ToString для зручного відображення у простих списках.
        /// </summary>
        public override string ToString()
        {
            return $"{Title} ({Author}) - {Year}";
        }
    }
}