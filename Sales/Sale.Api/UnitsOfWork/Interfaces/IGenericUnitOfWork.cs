using Sale.Share.DTOs;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Interfaces
{
    public interface IGenericUnitOfWork<T> where T : class
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<IEnumerable<T>>> GetAsync();
        Task<ActionResponse<IEnumerable<T>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<ActionResponse<T>> AddAsync(T model);
        Task<ActionResponse<T>> UpdateAsync(T model);
        Task<ActionResponse<T>> DeleteAsync(int id);
        Task<ActionResponse<T>> GetAsync(int id);
    }
}
