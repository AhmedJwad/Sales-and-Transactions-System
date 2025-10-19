using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Enums;
using Sale.Share.Responses;
using System.Threading.Tasks;

namespace Sale.Api.Repositories.Implementations
{
    public class OrdersRepository : GenericRepository<Order>, IOrdersRepository
    {
        private readonly DataContext _context;
        private readonly IUsersRepository _usersRepository;

        public OrdersRepository(DataContext context, IUsersRepository usersRepository) : base(context)
        {
            _context = context;
            _usersRepository = usersRepository;
        }

        public async Task<ActionResponse<IEnumerable<OrderResponseDTO>>> GetAsync(string email, PaginationDTO pagination)
        {
            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<IEnumerable<OrderResponseDTO>>()
                {
                    WasSuccess = false,
                    Message = "user does not exist"
                };
            }
            var queryable = _context.orders.Include(u => u.User!).Include(o => o.OrderDetails!).ThenInclude(p => p.Product).AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.User!.FirstName.ToLower().Contains(pagination.Filter.ToLower()));
            }
            var isAdmin = await _usersRepository.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(u => u.User!.Email == email);
            }
            var result=queryable.Select(o=> new OrderResponseDTO
            {
                Id=o.Id,
                Date=o.Date,
                Remarks=o.Remarks,
                UserFullName=o.User!.FirstName +" "+ o.User.LastName,
                UserEmail=o.User!.Email,
                UserPhoto=o.User!.Photo,
                Lines=o.Lines,
                Quantity=(int)o.Quantity,
                Value=o.Value,
                orderStatus=o.OrderStatus,
                orderDetailResponseDTOs=o.OrderDetails!.Select(od=> new OrderDetailResponseDTO
                {
                    Id=od.Id,
                    Description=od.Description,
                    Image=od.Image,
                    Name=od.Name,
                    Price=od.Price,
                    Quantity=(int)od.Quantity,
                    Value=od.Value,

                }).ToList(),                
            }).ToList();
            return new ActionResponse<IEnumerable<OrderResponseDTO>>
            {
                WasSuccess = true,
                Result = result.OrderByDescending(o=>o.Date),
            };

        }

        public  async Task<ActionResponse<OrderResponseDTO>> GetAsync(int id)
        {
            var order = await _context.orders.Include(u => u.User!)
                        .ThenInclude(c => c.City!).ThenInclude(s => s.State!).ThenInclude(cn => cn.Country!)
                        .Include(o => o.OrderDetails!).ThenInclude(p => p.Product).ThenInclude(pi => pi.ProductImages)
                        .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return new ActionResponse<OrderResponseDTO>
                {
                    WasSuccess = false,
                    Message = "order does not exist"
                };
            }
            var orderDTO = new OrderResponseDTO
            {
                Id = order.Id,
                Date = order.Date,
                Remarks = order.Remarks,
                UserFullName = order.User!.FirstName + " " + order.User.LastName,
                UserEmail = order.User!.Email,
                UserPhoto = order.User!.Photo ?? "/no-image.png",
                Lines = order.Lines,
                Quantity = (int)order.Quantity,
                Value = order.Value,
                orderStatus = order.OrderStatus,
                orderDetailResponseDTOs = order.OrderDetails!.Select(od => new OrderDetailResponseDTO
                {
                    Id = od.Id,
                    Name = od.Name,
                    Description = od.Description,
                    Price = od.Price,
                    Quantity = (int)od.Quantity,
                    Value = od.Value,
                    Image = od.Image ?? "/no-image.png"
                }).ToList()
            };
            return new ActionResponse<OrderResponseDTO>
            {
                WasSuccess = true,
                Result = orderDTO
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(string email, PaginationDTO pagination)
        {
            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<int>
                {
                    WasSuccess = false,
                    Message = "user does not exist"
                };
            }
            var queryable = _context.orders.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.User!.FirstName.ToLower().Contains(pagination.Filter.ToLower()));
            }
            var isAdmin = await _usersRepository.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(x => x.User!.Email == email);
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = (int)totalPages
            };
        }

        public async Task<ActionResponse<Order>> UpdateFullAsync(string email, OrderDTO orderDTO)
        {
            var user = await _usersRepository.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "user does not exist"
                };
            }
            var isAdmin = await _usersRepository.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin && orderDTO.OrderStatus != OrderStatus.Cancelled)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "Only allowed for administrators."
                };
            }
            var order = await _context.orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(x => x.Id == orderDTO.Id);
            if (order == null)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "order does not exist"
                };
            }
            if (orderDTO.OrderStatus == OrderStatus.Cancelled)
            {
                await ReturnStockAsync(order);
            }
            order.OrderStatus = orderDTO.OrderStatus;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return new ActionResponse<Order>
            {
                WasSuccess = true,
                Result = order,
            };        
         }
        

        private async Task ReturnStockAsync(Order order)
        {
            foreach (var orderDetails in order.OrderDetails!)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderDetails.ProductId);
                if(product !=null)
                {
                    product.Stock +=(decimal)orderDetails.Quantity;
                }                
            }
            await _context.SaveChangesAsync();
           
        }
        public async Task<ActionResponse<IEnumerable<Order>>> GetReportAsync(DatesDTO datesDTO)
        {
            var queryable = _context.orders.Where(x => x.OrderStatus != OrderStatus.Cancelled && x.Date >= datesDTO.InitialDate &&
            x.Date <= datesDTO.FinalDate).Include(x => x.User).Include(x => x.OrderDetails!).
            ThenInclude(x => x.Product).AsQueryable();
            var orders = await queryable
                    .OrderBy(x => x.Date)
                    .ToListAsync();
            return new ActionResponse<IEnumerable<Order>>
            {
                WasSuccess = true,
                Result = orders
            };
        }

    }
}
