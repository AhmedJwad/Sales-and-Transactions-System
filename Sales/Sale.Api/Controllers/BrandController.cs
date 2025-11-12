using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sale.Api.UnitsOfWork.Implementations;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;

namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class BrandController : GenericController<Brand>
    {
        private readonly IbrandUnitofWork _brandUnitofWork;

        public BrandController(IGenericUnitOfWork<Brand> genericUnitOfWork , IbrandUnitofWork brandUnitofWork) : base(genericUnitOfWork)
        {
           _brandUnitofWork = brandUnitofWork;
        }
        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _brandUnitofWork.GetRecordsNumberAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        [AllowAnonymous]
        [HttpGet("combo/{categoryId:int}")]
        public async Task<IActionResult> GetComboAsync(int subcategoryId, string lang = "en")
        {
            return Ok(await _brandUnitofWork.GetComboAsync(subcategoryId, lang));
        }

        [HttpGet]       
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _brandUnitofWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]      
        public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var action = await _brandUnitofWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }
        [AllowAnonymous]
        [HttpGet("combo")]
        public async Task<IActionResult> GetComboAsync(string lang = "en")
        {
            return Ok(await _brandUnitofWork.GetComboAsync(lang));
        }
        [HttpPost("full")]
        public async Task<IActionResult> PostFullAsync(BrandDTO brandDTO)
        {
            var action = await _brandUnitofWork.AddFullAsync(brandDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }

        [HttpPut("full")]
        public async Task<IActionResult> PutFullAsync(BrandDTO brandDTO)
        {
            var action = await _brandUnitofWork.UpdateFullAsync(brandDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }
        [HttpDelete("{id}")]
        public override async Task<IActionResult> DeleteAsync(int id)
        {
            var action = await _brandUnitofWork.DeleteAsync(id);
            if (!action.WasSuccess)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
