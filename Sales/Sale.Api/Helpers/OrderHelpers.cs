using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.UnitsOfWork.Implementations;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Enums;
using Sale.Share.Responses;

namespace Sale.Api.Helpers
{
    public class OrderHelpers : IorderHelper
    {
        private readonly DataContext _context;
        private readonly IUsersUnitOfWork _usersUnitOfWork;
        private readonly IorderUnitofWorks _orderUnitofWorks;
        private readonly IProductsUnitofWork _productsUnitofWork;

        public OrderHelpers(DataContext context,  IUsersUnitOfWork usersUnitOfWork, IorderUnitofWorks orderUnitofWorks , IProductsUnitofWork productsUnitofWork)
        {
            _context = context;
            _usersUnitOfWork = usersUnitOfWork;
           _orderUnitofWorks = orderUnitofWorks;
           _productsUnitofWork = productsUnitofWork;
        }
        public async Task<ActionResponse<bool>> ProcessOrderAsync(string email, OrderDTO orderDTO)
        {
           
            var user = await _usersUnitOfWork.GetUserAsync(email);
            if (user == null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "User does not exist"
                };
            }             
                       
            if (orderDTO.OrderDetails == null || !orderDTO.OrderDetails.Any())
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message= "There is no detail in the order"
                };
            }            
           
            var productIds = orderDTO.OrderDetails.Select(d => d.ProductId).ToList();
            var productsResponse = await _productsUnitofWork.GetProductsByIdsAsync(productIds);
            if (!productsResponse.WasSuccess || productsResponse.Result == null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "Some products could not be retrieved"
                };
            }               

            var products = productsResponse.Result;
           
            foreach (var detail in orderDTO.OrderDetails)
            {
                var product = products.FirstOrDefault(p => p.Id == detail.ProductId);
                if (product == null)
                {
                    return new ActionResponse<bool>
                    {
                        WasSuccess = false,
                        Message = $"The product {detail.ProductId} is no longer available"
                    };
                }

                if (product.Stock < (decimal)detail.Quantity)
                    return new ActionResponse<bool>
                    {
                        WasSuccess = false,
                        Message = $"Sorry we do not have enough stock of the product {product.ProductTranslations!.FirstOrDefault()!.Name}. " +
                        $"Please reduce the quantity or replace it with another."
                    };
                product.Stock -= (decimal)detail.Quantity;
              await  _productsUnitofWork.UpdateAsync(product);
            }
                       
            var order = new Order
            {
                Date = DateTime.UtcNow,
                User = user,
                Remarks = orderDTO.Remarks,
                OrderStatus = OrderStatus.New,
                OrderDetails = orderDTO.OrderDetails.Select(item =>
                {
                    var product = products.First(p => p.Id == item.ProductId);
                    return new OrderDetail
                    {
                        //Price = product.Price,                       
                        Image = product.MainImage!,
                        Quantity = item.Quantity,
                        Remarks = orderDTO.Remarks,
                        ProductId = item.ProductId                        
                    };
                }).ToList()
            };
            
            await _orderUnitofWorks.AddAsync(order);
            return new ActionResponse<bool>
            {
                WasSuccess = true,
                Message = "Order placed successfully",
                Result = true
            };

           
        }

    }
}
