using Sale.Share.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class SerialNumber
    {
        public int Id { get; set; }
        public int ProductId { get; set; } 
        public string SerialNumberValue { get; set; } = null!;
        public DateTime PurchaseDate { get; set; }
        public SerialStatus SerialStatus { get; set; }
        public Product? Product { get; set; }
    }
}
