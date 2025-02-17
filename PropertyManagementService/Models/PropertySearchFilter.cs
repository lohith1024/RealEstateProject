using System.Collections.Generic;

namespace PropertyManagementService.Models
{
    public class PropertySearchFilter
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int? MinBedrooms { get; set; }
        public int? MaxBedrooms { get; set; }
        public int? MinBathrooms { get; set; }
        public int? MaxBathrooms { get; set; }
        public decimal? MinSquareFootage { get; set; }
        public decimal? MaxSquareFootage { get; set; }
        public PropertyType? PropertyType { get; set; }
        public PropertyStatus? PropertyStatus { get; set; }
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool? SortDescending { get; set; }
    }
}