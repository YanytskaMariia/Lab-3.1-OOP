using System.Collections.Generic;
using LinkCollector.Models;

namespace LinkCollector.Services
{
    /// <summary>
    /// Contract for generating citation text for ResourceLink objects.
    /// </summary>
    public interface ICitationService
    {
        string GenerateCitation(ResourceLink link, CitationStyle style);
        string GenerateList(List<ResourceLink> links, CitationStyle style);
    }
}