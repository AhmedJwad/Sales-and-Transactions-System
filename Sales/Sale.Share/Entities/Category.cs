using Sale.Share.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class Category 
    {
        public int Id { get; set; }
      
        [Display(Name = "Photo")]
        public string? Photo { get; set; }
        public ICollection<Subcategory>? subcategories { get; set; }
        public ICollection<CategoryTranslation>? categoryTranslations { get; set; }
        
    }
}
