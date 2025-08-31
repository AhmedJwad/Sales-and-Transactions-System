using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class ProductColor
    {
        public int Id { get; set; }
        public int ColorId { get; set; }
        public Colour? color { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        
    }
}
