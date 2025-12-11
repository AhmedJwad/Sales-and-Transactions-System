using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.Entities;

namespace Sale.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class CurrenciesController : GenericController<Currency>
    {
        public CurrenciesController(IGenericUnitOfWork<Currency> genericUnitOfWork) : base(genericUnitOfWork)
        {

        }
    }
}
