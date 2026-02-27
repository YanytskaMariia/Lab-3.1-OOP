using System.Collections.Generic;
using LinkCollector.Models;

namespace LinkCollector.Services
{
    /// <summary>
    /// Репозиторій для керування посиланнями (порт для збереження/отримання даних).
    /// </summary>
    public interface ILinkRepository
    {
        List<ResourceLink> GetAll();
        List<ResourceLink> Search(string query);
        void Add(ResourceLink link);
        void Remove(ResourceLink link);

        List<string> GetCategories();
        void AddCategory(string cat);
        void RemoveCategory(string cat);
    }
}