using System.Collections.Generic;
using System.Threading.Tasks;
using SearchService.Models;

namespace SearchService.Interfaces
{
    public interface ISearchService
    {
        Task<IEnumerable<Property>> SearchProperties(SearchCriteria criteria);
    }
}