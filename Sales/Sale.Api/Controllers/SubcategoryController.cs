using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;

namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcategoryController : GenericController<Subcategory>
    {
        private readonly iSubcategoriesUnitofWorks _iSubcategoriesUnitofWorks;

        public SubcategoryController(IGenericUnitOfWork<Subcategory> genericUnitOfWork , iSubcategoriesUnitofWorks iSubcategoriesUnitofWorks) : base(genericUnitOfWork)
        {
          _iSubcategoriesUnitofWorks = iSubcategoriesUnitofWorks;
        }
        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination )
        {
            var response=await _iSubcategoriesUnitofWorks.GetRecordsNumberAsync(pagination);
            if(response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        [AllowAnonymous]
        [HttpGet("combo")]
        public async Task<IActionResult> GetComboAsync()
        {
            return Ok(await _iSubcategoriesUnitofWorks.GetComboAsync());
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _iSubcategoriesUnitofWorks.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var action = await _iSubcategoriesUnitofWorks.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

    }
}
