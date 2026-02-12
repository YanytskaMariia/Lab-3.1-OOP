namespace LinkCollector.Models
{
    /// <summary>
    /// Тип ресурсу (Книга, Веб-сайт тощо).
    /// </summary>
    public enum LinkType
    {
        Book,           
        WebResource,    
        Video,          
        Article         
    }

    /// <summary>
    /// Стандарти цитування для генерації тексту.
    /// </summary>
    public enum CitationStyle
    {
        DSTU_8302,  // ДСТУ 8302:2015 (Україна)
        Harvard,    // Гарвардський стиль
        BibTeX      // Формат для LaTeX
    }
}