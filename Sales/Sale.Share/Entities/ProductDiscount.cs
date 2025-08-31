using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class ProductDiscount
    {
        public int Id { get; set; }
        public int discountId { get; set; }
        public Discount discount { get; set; }
        public int productID { get; set; }
        public Product product { get; set; }

    }
}
