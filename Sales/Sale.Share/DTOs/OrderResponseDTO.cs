using Sale.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Remarks { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhoto { get; set; }
        public int Lines { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
        public OrderStatus orderStatus { get; set; } 
        public List<OrderDetailResponseDTO>? orderDetailResponseDTOs { get; set; }
    }
}
