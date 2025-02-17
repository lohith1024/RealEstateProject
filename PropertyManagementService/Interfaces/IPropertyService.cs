using System.Collections.Generic;
using System.Threading.Tasks;
using PropertyManagementService.Models;

namespace PropertyManagementService.Interfaces
{
    public interface IPropertyService
    {
        Task<Property> AddProperty(Property property);
        Task<bool> DeleteProperty(int propertyId);
        Task<Property?> UpdateProperty(int propertyId, Property property);
        Task<Property?> GetPropertyDetails(int propertyId);
        Task<List<Property>> GetAllProperties(PropertySearchFilter filter);
        Task<bool> UpdatePropertyStatus(int propertyId, PropertyStatus status);
        Task<List<Property>> GetUserProperties(int userId);
        Task<List<Property>> SearchProperties(string searchTerm);
    }
}