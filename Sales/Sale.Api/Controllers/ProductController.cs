using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Sale.Api.UnitsOfWork.Implementations;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Response;

namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ProductController : GenericController<Product>
    {
        private readonly IProductsUnitofWork _productsUnitOfWork;

        public ProductController(IGenericUnitOfWork<Product> genericUnitOfWork , IProductsUnitofWork productsUnitofWork) : base(genericUnitOfWork)
        {
           _productsUnitOfWork = productsUnitofWork;
        }
        [AllowAnonymous]
        [HttpGet("combo")]
        public async Task<IActionResult> GetComboAsync(string lang = "en")
        {
            return Ok(await _productsUnitOfWork.GetComboAsync(lang));
        }
        [AllowAnonymous]
        [HttpGet("recordsNumber")]        
        public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _productsUnitOfWork.GetRecordsNumberAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]       
        public override async Task<IActionResult> DeleteAsync(int id)
        {
            var action = await _productsUnitOfWork.DeleteAsync(id);
            if (!action.WasSuccess)
            {
                return NotFound(action.Message);
            }
            return Ok(action);
        }

        [HttpPost("addImages")]
        public async Task<IActionResult> PostAddImagesAsync(ImageDTO imageDTO)
        {
            var action = await _productsUnitOfWork.AddImageAsync(imageDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpPost("removeLastImage")]
        public async Task<IActionResult> PostRemoveLastImageAsync(ImageDTO imageDTO)
        {
            var action = await _productsUnitOfWork.RemoveLastImageAsync(imageDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [AllowAnonymous]
        [HttpGet]      
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _productsUnitOfWork.GetAsyncProduct(pagination);
            if (response.WasSuccess)
            {        
                return Ok(response.Result);
            }
            return BadRequest();
        }
      

        [AllowAnonymous]
        [HttpGet("totalPages")]        
        public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var action = await _productsUnitOfWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var action = await _productsUnitOfWork.GetAsync(id);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }

        [HttpPost("full")]
        public async Task<IActionResult> PostFullAsync(ProductDTO productDTO)
        {
            var action = await _productsUnitOfWork.AddFullAsync(productDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }

        [HttpPut("full")]
        public async Task<IActionResult> PutFullAsync(ProductDTO productDTO)
        {
            var action = await _productsUnitOfWork.UpdateFullAsync(productDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }
        [HttpGet("getProductbyCategory")]
        public async Task<IActionResult> GetProductCountByCategoryAsync(string lang = "en")
        {
            return Ok(await _productsUnitOfWork.GetProductCountByCategoryAsync(lang));
        }
        [AllowAnonymous]
        [HttpGet("getproductbysubcategory/{subcategoryId}")]
        public async Task<IActionResult>GetProductbySubcategory(int subcategoryId)
        {
            var result=await _productsUnitOfWork.GetProductsBySubcategoryAsync(subcategoryId);
            if(result.WasSuccess)
            {
                return Ok(result.Result);
            }
            return NotFound(result.Message);
        }

        [AllowAnonymous]
        [HttpPost("productfilter")]
        public async Task<IActionResult> GetProductbyfilter([FromBody] ProductFilterDto productFilterDto)
        {
            var result = await _productsUnitOfWork.FilterProducts(productFilterDto);
            if (result.WasSuccess)
            {
                return Ok(result.Result);
            }
            return NotFound(result.Message);
        }

        [AllowAnonymous]
        [HttpGet("getfullproduct")]
        public async Task<IActionResult>GetFullProduct()
        {
            var result = await _productsUnitOfWork.GetfullProduct();
            if (result.WasSuccess)
            {
                return Ok(result.Result);
            }
            return NotFound(result.Message);
        }
    }
}
