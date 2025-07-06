using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class ProductsubCategory
    {
        public int Id { get; set; }
        public int subcategoryId { get; set; }
        public Subcategory? Category { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
