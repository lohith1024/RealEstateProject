using Microsoft.AspNetCore.Mvc;
using PropertyManagementService.Models;
using PropertyManagementService.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropertyManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Property>>> GetAllProperties([FromQuery] PropertySearchFilter filter)
        {
            var properties = await _propertyService.GetAllProperties(filter);
            return Ok(properties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Property>> GetPropertyById(int id)
        {
            var property = await _propertyService.GetPropertyDetails(id);
            if (property == null)
                return NotFound();
            return Ok(property);
        }

        [HttpPost]
        public async Task<ActionResult<Property>> AddProperty([FromBody] Property property)
        {
            var result = await _propertyService.AddProperty(property);
            return CreatedAtAction(nameof(GetPropertyById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Property>> UpdateProperty(int id, [FromBody] Property property)
        {
            var result = await _propertyService.UpdateProperty(id, property);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProperty(int id)
        {
            var result = await _propertyService.DeleteProperty(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdatePropertyStatus(int id, [FromBody] PropertyStatus status)
        {
            var result = await _propertyService.UpdatePropertyStatus(id, status);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Property>>> GetUserProperties(int userId)
        {
            var properties = await _propertyService.GetUserProperties(userId);
            return Ok(properties);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Property>>> SearchProperties([FromQuery] string searchTerm)
        {
            var properties = await _propertyService.SearchProperties(searchTerm);
            return Ok(properties);
        }
    }
}