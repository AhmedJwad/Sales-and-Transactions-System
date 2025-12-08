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
    public class ExchangeRateController : GenericController<ExchangeRate>
    {
        private readonly IexchangeRateUnitofWork _exchangeRateUnitofWork;

        public ExchangeRateController(IGenericUnitOfWork<ExchangeRate> genericUnitOfWork,
            IexchangeRateUnitofWork exchangeRateUnitofWork) : base(genericUnitOfWork)
        {
           _exchangeRateUnitofWork = exchangeRateUnitofWork;
        }
        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _exchangeRateUnitofWork.GetRecordsNumberAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
        [AllowAnonymous]
        [HttpGet("combo")]
        public async Task<IActionResult> GetComboAsync()
        {
            return Ok(await _exchangeRateUnitofWork.GetComboAsync());
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _exchangeRateUnitofWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var action = await _exchangeRateUnitofWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }
       
        [HttpPost("full")]
        public async Task<IActionResult> PostFullAsync(ExchangeRateDTO exchangeRateDTO)
        {
            var action = await _exchangeRateUnitofWork.AddFullAsync(exchangeRateDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }
              
       
        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var action = await _exchangeRateUnitofWork.GetAsync(id);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }

    }
}
