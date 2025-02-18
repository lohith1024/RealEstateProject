using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagementService.Models;

namespace UserManagementService.Interfaces
{
    public interface IPropertyService
    {
        Task<List<Property>> GetUserPropertiesAsync(int userId);
        Task<Property?> GetPropertyDetailsAsync(int propertyId);
    }
}