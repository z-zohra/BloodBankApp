using AutoMapper;
using BloodBank.DTOs;
using BloodBank.Interfaces;
using BloodBank.Models;
using Microsoft.AspNetCore.Mvc;

namespace BloodBankAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonationCenterController : ControllerBase
    {
        private readonly IDonationCenterRepository _repository;
        private readonly IMapper _mapper;

        public DonationCenterController(IDonationCenterRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        //http://localhost:5031/api/DonationCenter
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var centers = await _repository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<DonationCenterReadDto>>(centers));
        }
        // http://localhost:5031/api/DonationCenter/ON_001/London
        [HttpGet("{id}")]
        public async Task<ActionResult<DonationCenterReadDto>> GetById(string id)
        {
            var center = await _repository.GetByIdAsync(id);
            if (center == null) return NotFound();
            return Ok(_mapper.Map<DonationCenterReadDto>(center));
        }

        // POST
        // http://localhost:5031/api/DonationCenter
        [HttpPost]
        public async Task<IActionResult> AddDonationCenter([FromBody] DonationCenterCreateDto donationCenterDto)
        {
            if (donationCenterDto == null)
                return BadRequest("Donation center data is required.");

            // Map DTO to model
            var donationCenter = _mapper.Map<DonationCenter>(donationCenterDto);

            // Add the donation center to DynamoDB
            await _repository.AddAsync(donationCenter);

            return CreatedAtAction(nameof(GetById), new { id = donationCenter.Id }, donationCenter);
        }

        // PUT
        // http://localhost:5031/api/DonationCenter/ON_001
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDonationCenter(string id, [FromBody] DonationCenterUpdateDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("Id in URL doesn't match Id in request body.");
            }

            try
            {
                await _repository.UpdateAsync(id, updateDto);
                return NoContent(); // 204 No Content: successful update with no content returned
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // If no fields are provided for update
            }
            catch (Exception ex)
            {
                // Log the exception 
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // 500 Internal Server Error
            }
        }
        // PATCH
        //
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateHoursOfOperation(string id, [FromBody] DonationCenterUpdateHoursDto updateHoursDto)
        {
            try
            {
                await _repository.UpdateHoursAsync(id, updateHoursDto);
                return NoContent(); // HTTP 204
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = $"Donation center with Id '{id}' not found." }); // HTTP 404
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message }); // HTTP 500
            }
        }


        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonationCenter(string id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return NoContent(); // HTTP 204
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message }); // HTTP 404
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message }); // HTTP 500
            }
        }


    }

}
