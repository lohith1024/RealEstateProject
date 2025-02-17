using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PropertyManagementService.Data;
using PropertyManagementService.Interfaces;
using PropertyManagementService.Models;

namespace PropertyManagementService.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly ApplicationDbContext _context;

        public PropertyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Property> AddProperty(Property property)
        {
            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();
            return property;
        }

        public async Task<bool> DeleteProperty(int propertyId)
        {
            var property = await _context.Properties.FindAsync(propertyId);
            if (property == null) return false;

            property.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Property?> UpdateProperty(int propertyId, Property property)
        {
            var existingProperty = await _context.Properties.FindAsync(propertyId);
            if (existingProperty == null) return null;

            existingProperty.Title = property.Title;
            existingProperty.Description = property.Description;
            existingProperty.Price = property.Price;
            existingProperty.Address = property.Address;
            existingProperty.City = property.City;
            existingProperty.State = property.State;
            existingProperty.ZipCode = property.ZipCode;
            existingProperty.Bedrooms = property.Bedrooms;
            existingProperty.Bathrooms = property.Bathrooms;
            existingProperty.SquareFootage = property.SquareFootage;
            existingProperty.YearBuilt = property.YearBuilt;
            existingProperty.Type = property.Type;
            existingProperty.Status = property.Status;
            existingProperty.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingProperty;
        }

        public async Task<Property?> GetPropertyDetails(int propertyId)
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == propertyId && p.IsActive);
        }

        public async Task<List<Property>> GetAllProperties(PropertySearchFilter filter)
        {
            var query = _context.Properties.Where(p => p.IsActive);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            if (!string.IsNullOrEmpty(filter.City))
                query = query.Where(p => p.City.ToLower() == filter.City.ToLower());
            if (!string.IsNullOrEmpty(filter.State))
                query = query.Where(p => p.State.ToLower() == filter.State.ToLower());
            if (filter.MinBedrooms.HasValue)
                query = query.Where(p => p.Bedrooms >= filter.MinBedrooms.Value);
            if (filter.MaxBedrooms.HasValue)
                query = query.Where(p => p.Bedrooms <= filter.MaxBedrooms.Value);
            if (filter.MinBathrooms.HasValue)
                query = query.Where(p => p.Bathrooms >= filter.MinBathrooms.Value);
            if (filter.MaxBathrooms.HasValue)
                query = query.Where(p => p.Bathrooms <= filter.MaxBathrooms.Value);
            if (filter.MinSquareFootage.HasValue)
                query = query.Where(p => p.SquareFootage >= filter.MinSquareFootage.Value);
            if (filter.MaxSquareFootage.HasValue)
                query = query.Where(p => p.SquareFootage <= filter.MaxSquareFootage.Value);
            if (filter.PropertyType.HasValue)
                query = query.Where(p => p.Type == filter.PropertyType.Value);
            if (filter.PropertyStatus.HasValue)
                query = query.Where(p => p.Status == filter.PropertyStatus.Value);

            // Apply sorting
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                query = filter.SortBy.ToLower() switch
                {
                    "price" => filter.SortDescending == true ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    "date" => filter.SortDescending == true ? query.OrderByDescending(p => p.ListedDate) : query.OrderBy(p => p.ListedDate),
                    "bedrooms" => filter.SortDescending == true ? query.OrderByDescending(p => p.Bedrooms) : query.OrderBy(p => p.Bedrooms),
                    "bathrooms" => filter.SortDescending == true ? query.OrderByDescending(p => p.Bathrooms) : query.OrderBy(p => p.Bathrooms),
                    "squarefootage" => filter.SortDescending == true ? query.OrderByDescending(p => p.SquareFootage) : query.OrderBy(p => p.SquareFootage),
                    _ => query.OrderByDescending(p => p.ListedDate)
                };
            }
            else
            {
                query = query.OrderByDescending(p => p.ListedDate);
            }

            // Apply pagination
            if (filter.Page.HasValue && filter.PageSize.HasValue)
            {
                query = query.Skip((filter.Page.Value - 1) * filter.PageSize.Value)
                            .Take(filter.PageSize.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> UpdatePropertyStatus(int propertyId, PropertyStatus status)
        {
            var property = await _context.Properties.FindAsync(propertyId);
            if (property == null) return false;

            property.Status = status;
            property.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Property>> GetUserProperties(int userId)
        {
            return await _context.Properties
                .Where(p => p.OwnerId == userId && p.IsActive)
                .OrderByDescending(p => p.ListedDate)
                .ToListAsync();
        }

        public async Task<List<Property>> SearchProperties(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return new List<Property>();

            searchTerm = searchTerm.ToLower();
            return await _context.Properties
                .Where(p => p.IsActive &&
                    (p.Title.ToLower().Contains(searchTerm) ||
                     p.Description.ToLower().Contains(searchTerm) ||
                     p.Address.ToLower().Contains(searchTerm) ||
                     p.City.ToLower().Contains(searchTerm) ||
                     p.State.ToLower().Contains(searchTerm) ||
                     p.ZipCode.Contains(searchTerm)))
                .OrderByDescending(p => p.ListedDate)
                .ToListAsync();
        }
    }
}