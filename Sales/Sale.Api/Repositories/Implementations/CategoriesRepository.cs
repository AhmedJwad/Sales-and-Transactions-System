﻿using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class CategoriesRepository :GenericRepository<Category>, ICategoriesRepository
    {
        private readonly DataContext _context;

        public CategoriesRepository(DataContext context) : base(context)
        {
           _context = context;
        }

        public override async Task<ActionResponse<IEnumerable<Category>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Category>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync()
            };
        }

        public async Task<IEnumerable<Category>> GetComboAsync()
        {
            return await _context.Categories.OrderBy(x => x.Name).ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination?.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            int recordsNumber = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber,
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination?.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination!.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages,
            };
        }
    }
}
