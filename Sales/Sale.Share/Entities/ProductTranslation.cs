using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class ProductTranslation
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Language { get; set; } = null!;
        [Display(Name = "Product")]
        [MaxLength(500, ErrorMessage = "Field {0} cannot be longer than {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Name { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        [MaxLength(500, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        public string Description { get; set; } = null!;
        public Product? Product { get; set; }
    }
}
