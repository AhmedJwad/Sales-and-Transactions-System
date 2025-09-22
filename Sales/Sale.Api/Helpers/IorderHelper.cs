using Sale.Share.DTOs;
using Sale.Share.Responses;

namespace Sale.Api.Helpers
{
    public interface IorderHelper
    {
        Task<ActionResponse<bool>> ProcessOrderAsync(string email, OrderDTO orderDTO);
    }
}
