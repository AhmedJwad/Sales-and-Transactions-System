using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }    
        public string? Name { get; set; }    
        public string? photo { get; set; }
        public List<string>? Subcategories { get; set; }       
        public List<CategoryTranslationDto>? categoryTranslations { get; set; } 
    }
}
