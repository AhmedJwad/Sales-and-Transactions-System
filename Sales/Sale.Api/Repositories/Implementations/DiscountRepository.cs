using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class DiscountRepository :GenericRepository<Discount> ,IdiscountRepository
    {
        private readonly DataContext _context;

        public DiscountRepository(DataContext context) : base(context)
        {
           _context = context;
        }

        public async Task<ActionResponse<Discount>> AddFullAsync(DiscountDTO discountDTO)
        {
            try
            {
                var discount = new Discount
                {
                    DiscountPercent=discountDTO.DiscountPercent,
                    StartTime=discountDTO.StartTime,
                    Endtime=discountDTO.Endtime,
                    isActive=true,
                    productDiscounts = new List<ProductDiscount>()
                };
                var products = await _context.Products.Where(p => discountDTO.ProductIds!.Contains(p.Id)).ToListAsync();
                foreach (var product in products)
                {
                    if(!discount.productDiscounts!.Any(d=>d.productID==product.Id))
                        {
                        discount.productDiscounts!.Add(new ProductDiscount
                        {                            
                           product=product,
                        });
                        }
                }
                _context.Add(discount);
                await _context.SaveChangesAsync();
                return new ActionResponse<Discount>
                {
                    WasSuccess = true,
                    Result = discount
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<Discount>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };
                
            }
        }

        public override async Task<ActionResponse<Discount>> DeleteAsync(int id)
        {
            try
            {
                var discount = await _context.discounts.Include(p => p.productDiscounts!).ThenInclude(p => p.product)
                        .FirstOrDefaultAsync(d => d.Id == id);
                if (discount == null)
                {
                    return new ActionResponse<Discount>
                    {
                        WasSuccess = false,
                        Message = "Discount not found ."
                    };
                }
                _context.productDiscounts.RemoveRange(discount.productDiscounts!);
                _context.Remove(discount);
                await _context.SaveChangesAsync();
                return new ActionResponse<Discount>
                {
                    WasSuccess = true,
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<Discount>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public override async Task<ActionResponse<Discount>> GetAsync(int id)
        {
            var discount = await _context.discounts.Include(pd => pd.productDiscounts!).ThenInclude(p => p.product)
                 .FirstOrDefaultAsync(d => d.Id == id);
            if(discount==null)
            {
                return new ActionResponse<Discount>
                {
                    WasSuccess= false,
                    Message= "Discount not found ."
                };
            }
            return new ActionResponse<Discount>
            {
                WasSuccess= true,
                Result= discount
            };
        }

        public override async Task<ActionResponse<IEnumerable<Discount>>> GetAsync(PaginationDTO pagination)
        {
           var queryable = _context.discounts.Include(pd => pd.productDiscounts!).ThenInclude(p => p.product)
                .ThenInclude(p=>p.ProductTranslations!.Where(p=>p.Language.ToLower()==pagination.Language!.ToLower()))
                .AsQueryable();
            // Apply filtering if needed
            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(d => d.DiscountPercent.ToString().Contains(pagination.Filter) ||
                                                 d.StartTime.ToString().Contains(pagination.Filter) ||
                                                 d.Endtime.ToString().Contains(pagination.Filter));
            }
          
            return new ActionResponse<IEnumerable<Discount>>
            {
                WasSuccess = true,
                Result = await queryable.Paginate(pagination).ToListAsync(),
            };  
        }

        public async Task<IEnumerable<Discount>> GetComboAsync()
        {
            return await _context.discounts.Include(dp=>dp.productDiscounts!).ThenInclude(p=>p.product).ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO)
        {
           var queryable= _context.discounts.Include(d=>d.productDiscounts!).ThenInclude(pd=>pd.product).AsQueryable();
            // Apply filtering if needed
            if (!string.IsNullOrEmpty(paginationDTO.Filter))
            {
                queryable = queryable.Where(d => d.DiscountPercent.ToString().Contains(paginationDTO.Filter) ||
                                                 d.StartTime.ToString().Contains(paginationDTO.Filter) ||
                                                 d.Endtime.ToString().Contains(paginationDTO.Filter));
            }
            int count = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = count
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO)
        {
            var queryable = _context.discounts.Include(d => d.productDiscounts!).ThenInclude(pd => pd.product).AsQueryable();
            // Apply filtering if needed
            if (!string.IsNullOrEmpty(paginationDTO.Filter))
            {
                queryable = queryable.Where(d => d.DiscountPercent.ToString().Contains(paginationDTO.Filter) ||
                                                 d.StartTime.ToString().Contains(paginationDTO.Filter) ||
                                                 d.Endtime.ToString().Contains(paginationDTO.Filter));
            }
            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count/ paginationDTO.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }
        public async Task<ActionResponse<Discount>> UpdateFullAsync(DiscountDTO discountDTO)
        {
            try
            {
                var discout=await _context.discounts.Include(p => p.productDiscounts!).ThenInclude(p => p.product)
                        .FirstOrDefaultAsync(d => d.Id == discountDTO.Id);
                if(discout==null)
                {
                    return new ActionResponse<Discount>
                    {
                        WasSuccess = false,
                        Message = "Discount not found ."
                    };
                }
                _context.productDiscounts.RemoveRange(discout.productDiscounts!);
                discout.productDiscounts=new List<ProductDiscount>();
                discout.DiscountPercent= discountDTO.DiscountPercent;
                discout.StartTime= discountDTO.StartTime;
                discout.Endtime= discountDTO.Endtime;
                discout.isActive= discountDTO.isActive;
                var discont=await _context.Products.Where(d=> discountDTO.ProductIds!.Contains(d.Id)).ToListAsync();
                foreach (var product in discont)
                {
                    _context.productDiscounts.Add(new ProductDiscount
                    {
                        discountId=discout.Id,
                        productID=product.Id,
                    });
                }

                _context.discounts.Update(discout);
                await _context.SaveChangesAsync();
                return new ActionResponse<Discount>
                {
                    WasSuccess = true,
                    Result = discout
                };

            }
            catch (Exception ex)
            {

                return new ActionResponse<Discount>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
