using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class CategoryTranslation
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Language { get; set; } = null!;
        [Display(Name = "Category")]
        [MaxLength(100, ErrorMessage = "Field {0} cannot be longer than {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Name { get; set; } = null!;       
        public Category? Category { get; set; } 
    }
}
