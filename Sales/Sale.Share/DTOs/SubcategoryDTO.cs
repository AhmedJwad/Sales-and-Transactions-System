using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
   public class SubcategoryDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [Display(Name = "Photo")]
        public string? Photo { get; set; }
        public int CategoryId { get; set; }
        public List<ProductDTO> Products { get; set; } = new();
        public List<SubcategoryTranslationDto>? SubcategoryTranslations { get; set; } 
    }
}
