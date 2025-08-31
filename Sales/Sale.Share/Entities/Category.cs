using Sale.Share.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class Category : IEntityWithName
    {
        public int Id { get; set; }

        [Display(Name = "Category")]
        [MaxLength(100, ErrorMessage = "Field {0} cannot be longer than {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Name { get; set; } = null!;
        [Display(Name = "Photo")]
        public string? Photo { get; set; }
        public ICollection<Subcategory>? subcategories { get; set; }

        
    }
}
