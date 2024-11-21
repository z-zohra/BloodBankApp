using AutoMapper;
using BloodBank.Interfaces;
using BloodBank.DTOs;
using Microsoft.AspNetCore.Mvc;
using BloodBank.Models;

namespace BloodBankAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // http://localhost:5031/api/BloodType
    public class BloodTypeController : Controller
    {
        private readonly IBloodTypeRepository _repository;
        private readonly IMapper _mapper;
        public BloodTypeController(IBloodTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        // GET ALL
        // http://localhost:5031/api/BloodType/ON_001
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllBloodTypes(string id)
        {
            var bloodTypes = await _repository.GetAllAsync(id);
            if (bloodTypes == null || !bloodTypes.Any()) return NotFound($"No blood types found for Id: {id}");

            return Ok(_mapper.Map<IEnumerable<BloodTypeReadDto>>(bloodTypes));
        }

        // GET BY ID
        // http://localhost:5031/api/BloodType/ON_001/B+
        [HttpGet("{id}/{bloodType}")]
        public async Task<IActionResult> GetBloodTypeById(string id, string bloodType)
        {
            var bloodTypeInfo = await _repository.GetByIdAsync(id,bloodType);
            if (bloodTypeInfo == null) return NotFound($"Blood type {bloodType} not found for Id: {id}");

            return Ok(_mapper.Map<BloodTypeReadDto>(bloodTypeInfo));
        }

        // POST
        // http://localhost:5031/api/BloodType/ON_001
        [HttpPost("{id}")]
        public async Task<IActionResult> AddBloodTypeAsync(string id, [FromBody] BloodTypeCreateDto bloodTypeDto)
        {
            var newBloodType = _mapper.Map<BloodTypeInfo>(bloodTypeDto);
            await _repository.AddBloodTypeAsync(id, newBloodType);
            return Ok(newBloodType);
        }

        // PUT
        //http://localhost:5031/api/BloodType/ON_001
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBloodType(string id,[FromBody] BloodTypeCreateDto bloodTypeDto)
        {
            var newBloodType = _mapper.Map<BloodTypeInfo>(bloodTypeDto);
            await _repository.UpdateBloodTypeAsync(id, newBloodType);
            return Ok(newBloodType);
        }

        // PATCH
        // http://localhost:5031/api/BloodType/ON_001/O+
        [HttpPatch("{id}/{bloodType}")]
        public async Task<IActionResult> UpdateBloodTypeStockLevel(string id, string bloodType, [FromBody] BloodTypeStockLevelDto stockLevelDto)
        {
            try
            {
                await _repository.UpdateStockLevelAsync(id, bloodType, stockLevelDto.StockLevel);
 
                return Ok(new { message = "Blood type stock level updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE
        // http://localhost:5031/api/BloodType/ON_001/O+
        [HttpDelete("{id}/{bloodType}")]
        public async Task<IActionResult> DeleteBloodType(string id, string bloodType)
        {
            try
            {
                await _repository.DeleteBloodTypeAsync(id, bloodType);

                return Ok(new { message = $"{bloodType} blood type removed successfully from the donation center." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
