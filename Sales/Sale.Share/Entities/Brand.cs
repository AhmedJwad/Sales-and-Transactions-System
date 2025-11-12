using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class Brand
    {
        public int Id { get; set; }       
        public int SubcategoryId { get; set; }
        public Subcategory? Subcategory { get; set; }
        public ICollection<Product>? Products { get; set; }
        public ICollection<BrandTranslation>? BrandTranslations { get; set; }   
    }
}
