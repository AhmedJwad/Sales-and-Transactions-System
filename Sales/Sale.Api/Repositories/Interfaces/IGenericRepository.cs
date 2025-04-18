﻿using Sale.Share.DTOs;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Interfaces
{
    public interface IGenericRepository<T>
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<T>> GetAsync(int Id);
        Task<ActionResponse<IEnumerable<T>>> GetAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<IEnumerable<T>>> GetAsync();
        Task<ActionResponse<T>> AddAsync(T entity);
        Task<ActionResponse<T>> UpdateAsync(T entity);
        Task<ActionResponse<T>> DeleteAsync(int id);
    }
}
