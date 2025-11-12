using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class BrandTranslation
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public string Language { get; set; } = null!;

        [Display(Name = "Brand")]
        [MaxLength(100, ErrorMessage = "Field {0} cannot be longer than {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Name { get; set; } = null!;
        public Brand?brand { get; set; }
    }
}
