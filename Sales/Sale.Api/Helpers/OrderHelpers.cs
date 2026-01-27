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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
               orderDTO.CurrencyCode ??= "IQ";
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
                        Message = "There is no detail in the order"
                    };
                }
               
                var productIds = orderDTO.OrderDetails.Select(d => d.ProductId).Distinct().ToList();
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
                var productsDict = products.ToDictionary(p => p.Id);                
                foreach (var detail in orderDTO.OrderDetails)
                {
                    if (!productsDict.TryGetValue(detail.ProductId, out var product))
                    {
                        return new ActionResponse<bool>
                        {
                            WasSuccess = false,
                            Message = $"The product {detail.ProductId} is no longer available"
                        };
                    }

                    var productName = product.ProductTranslations?.FirstOrDefault()?.Name ?? "Unknown product";

                    if (product.Stock < (decimal)detail.Quantity)
                    {
                        return new ActionResponse<bool>
                        {
                            WasSuccess = false,
                            Message = $"Sorry, we do not have enough stock of {productName}. " +
                                      "Please reduce the quantity or replace it with another."
                        };
                    }                  
                    product.Stock -= (decimal)detail.Quantity;
                }

               
                var order = new Order
                {
                    Date = DateTime.UtcNow,
                    UserId = user.Id, 
                    Remarks = orderDTO.Remarks,
                    OrderStatus = OrderStatus.New,
                    OrderDetails = new List<OrderDetail>()
                };

                foreach (var item in orderDTO.OrderDetails)
                {
                    var product = productsDict[item.ProductId];
                    
                    var price = product.ProductPrices?                      
                        .Where(pp => pp.Currency!.Code == orderDTO.CurrencyCode)
                        .OrderByDescending(pp => pp.CreatedAt)
                        .Select(pp => pp.Price)
                        .FirstOrDefault();

                    if (price == null || price <= 0)
                    {
                        return new ActionResponse<bool>
                        {
                            WasSuccess = false,
                            Message = $"Price for product {product.ProductTranslations?.FirstOrDefault()?.Name ?? "Unknown"} is not available"
                        };
                    }
                    var activeDiscount = product.productDiscount?
                                        .Select(pd => pd.discount)
                                        .Where(d =>
                                            d.isActive &&
                                            d.StartTime <= DateTime.UtcNow &&
                                            d.Endtime >= DateTime.UtcNow)
                                        .OrderByDescending(d => d.DiscountPercent)
                                        .FirstOrDefault();
                    var discountPercent = activeDiscount?.DiscountPercent ?? 0;
                    var finalPrice = discountPercent > 0
                                    ? price.Value - (price.Value * discountPercent / 100)
                                    : price.Value;


                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price =finalPrice,
                        DiscountPercent = discountPercent,
                        CurrencyCode = orderDTO.CurrencyCode,
                        Image = product.MainImage,
                        Remarks = orderDTO.Remarks,
                        Description=item.Description,
                        Name=item.Name,
                        ColorId = item.ColorId,
                        SizeId = item.SizeId,
                        ColorName = product.productColor?
                        .FirstOrDefault(pc => pc.ColorId == item.ColorId)?
                        .color?.HexCode,
                        SizeName = product.productSize?
                        .FirstOrDefault(ps => ps.SizeId == item.SizeId)?
                        .size?.Name
                    });
                }               
                await _orderUnitofWorks.AddAsync(order);
                await _context.SaveChangesAsync(); 
                await transaction.CommitAsync();

                return new ActionResponse<bool>
                {
                    WasSuccess = true,
                    Message = "Order placed successfully",
                    Result = true
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


    }
}
