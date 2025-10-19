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
    public class CategoriesController : GenericController<Category>
    {
        private readonly ICategoriesUnitOfWork _categoriesUnitOfWork;

        public CategoriesController(IGenericUnitOfWork<Category> genericUnitOfWork, ICategoriesUnitOfWork categoriesUnitOfWork) : base(genericUnitOfWork)
        {
           _categoriesUnitOfWork = categoriesUnitOfWork;
        }
        [HttpGet]       
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _categoriesUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        [HttpGet("recordsNumber")]        
        public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _categoriesUnitOfWork.GetRecordsNumberAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        [AllowAnonymous]
        [HttpGet("combo")]
        public async Task<IActionResult> GetComboAsync(string lang = "en")
        {
            return Ok(await _categoriesUnitOfWork.GetComboAsync(lang));
        }

        [HttpGet("totalPages")]       
        public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var action = await _categoriesUnitOfWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }
        [HttpPost("full")]
        public async Task<IActionResult> PostFullAsync(CategoryDTO categoryDTO)
        {
            var action = await _categoriesUnitOfWork.AddFullAsync(categoryDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }

        [HttpPut("full")]
        public async Task<IActionResult> PutFullAsync(CategoryDTO categoryDTO)
        {
            var action = await _categoriesUnitOfWork.UpdateFullAsync(categoryDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }
        [HttpDelete("{id}")]
        public override async Task<IActionResult> DeleteAsync(int id)
        {
            var action = await _categoriesUnitOfWork.DeleteAsync(id);
            if (!action.WasSuccess)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
