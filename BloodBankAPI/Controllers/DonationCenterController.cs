using AutoMapper;
using BloodBank.DTOs;
using BloodBank.Interfaces;
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
        [HttpGet("{id}/{area}")]
        public async Task<ActionResult<DonationCenterReadDto>> GetById(string id, string area)
        {
            var center = await _repository.GetByIdAsync(id, area);
            if (center == null) return NotFound();
            return Ok(_mapper.Map<DonationCenterReadDto>(center));
        }

        // Implement POST, PUT, PATCH, DELETE...
    }

}
