using Sale.Share.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class OrderDetailDTO
    {        
        public int ProductId { get; set; }       
        public string? Remarks { get; set; }
        public string Name { get; set; } = null!;       
        public string Description { get; set; } = null!;       
        public decimal Price { get; set; }       
        public string Image { get; set; } = null!;      
        public float Quantity { get; set; }
         

    }
}
