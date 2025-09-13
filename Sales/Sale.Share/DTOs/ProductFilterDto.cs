using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class ProductFilterDto
    {
        public List<int>? ColorIds { get; set; }
        public List<int>? SizeIds { get; set; }
        public int? BrandId { get; set; }  
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

    }
}
