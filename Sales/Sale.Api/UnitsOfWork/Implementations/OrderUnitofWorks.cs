using Sale.Api.Repositories.Implementations;
using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Implementations
{
    public class OrderUnitofWorks :GenericUnitOfWork<Order>, IorderUnitofWorks
    {
        private readonly IOrdersRepository _ordersRepository;

        public OrderUnitofWorks(IGenericRepository<Order> repository, IOrdersRepository ordersRepository) : base(repository)
        {
            _ordersRepository = ordersRepository;
        }
        public async Task<ActionResponse<IEnumerable<Order>>> GetAsync(string email, PaginationDTO pagination)
        => await _ordersRepository.GetAsync(email, pagination);
        public override async Task<ActionResponse<Order>> GetAsync(int id)
        =>await _ordersRepository.GetAsync(id);
        public async Task<ActionResponse<IEnumerable<Order>>> GetReportAsync(DatesDTO datesDTO)
        => await _ordersRepository.GetReportAsync(datesDTO);
        public async Task<ActionResponse<int>> GetTotalPagesAsync(string email, PaginationDTO pagination)
        => await _ordersRepository.GetTotalPagesAsync(email, pagination);
        public async  Task<ActionResponse<Order>> UpdateFullAsync(string email, OrderDTO orderDTO)
        => await _ordersRepository.UpdateFullAsync(email, orderDTO);


    }
}
