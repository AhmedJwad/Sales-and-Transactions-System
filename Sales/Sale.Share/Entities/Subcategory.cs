using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class Subcategory
    {
        public int Id { get; set; }      
        [Display(Name = "Photo")]
        public string? Photo { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<ProductsubCategory>? Prosubcategories { get; set; }
        public ICollection<Brand>? Brands { get; set; }
        public ICollection<SubcategoryTranslation>? SubcategoryTranslations { get; set; }   

        [Display(Name = "Products")]
        public int ProductCategoriesNumber => Prosubcategories == null || Prosubcategories.Count == 0 ? 0 : Prosubcategories.Count;
    }
}
