using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sale.Api.Helpers;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;

namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IorderHelper _orderHelper;
        private readonly IorderUnitofWorks _orderUnitofWorks;

        public OrdersController(IorderHelper orderHelper, IorderUnitofWorks orderUnitofWorks)
        {
            _orderHelper = orderHelper;
           _orderUnitofWorks = orderUnitofWorks;
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(OrderDTO orderDTO)
        {
            var response = await _orderHelper.ProcessOrderAsync(User.Identity!.Name!, orderDTO);
            if (response.WasSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _orderUnitofWorks.GetAsync(User.Identity!.Name!, pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var action = await _orderUnitofWorks.GetTotalPagesAsync(User.Identity!.Name!, pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = await _orderUnitofWorks.GetAsync(id);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return NotFound(response.Message);
        }
        [HttpPut]
        public async Task<IActionResult> PutAsync(OrderDTO orderDTO)
        {
            var response = await _orderUnitofWorks.UpdateFullAsync(User.Identity!.Name!, orderDTO);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest(response.Message);
        }

    }
}
