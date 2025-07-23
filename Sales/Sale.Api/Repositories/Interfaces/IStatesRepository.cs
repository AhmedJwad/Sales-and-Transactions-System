using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;
using System.Threading.Tasks;

namespace Sale.Api.Repositories.Interfaces
{
    public interface IStatesRepository
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<State>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<State>>> GetAsync();
        Task<ActionResponse<IEnumerable<State>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<IEnumerable<State>> GetComboAsync(int countryId);
    }
}
