using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class productSize
    {
        public int Id { get; set; }
        public int SizeId { get; set; }
        public Sizep? size { get; set; }
        public int productId { get; set; }  
        public Product? product { get; set; }
    }
}
