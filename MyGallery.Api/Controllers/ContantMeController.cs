using Microsoft.AspNetCore.Mvc;
using MyGallery.Domain;
using MyGallery.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MyGallery.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContantMeController : ControllerBase
    {
        private readonly IContantMeService _contantMeService;

        public ContantMeController(IContantMeService contantMeService)
        {
            _contantMeService = contantMeService;

        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ContantMe>>> GetaAllContantMe()
        {
            return Ok(await _contantMeService.GetAllAsync());
        }

        [HttpPost]
        public async Task<ActionResult<ContantMe>> PostContantMeItem(ContantMe item)
        {
            await _contantMeService.AddAsync(item);
            return Ok();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ContantMe>> GetContantMeItem(int id)
        {
            var item = await _contantMeService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

    }
}
