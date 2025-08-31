using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
   public class ProductColorImage
    {
        public int Id { get; set; }
        public int ColorId { get; set; }
        public Colour? color { get; set; }
        public int ProductImageId { get; set; }
        public ProductImage? productImage { get; set; }
    }
}
