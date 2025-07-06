using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class Supplier
    {
        public int Id { get; set; }

        [Display(Name = "Supplier Full Name")]
        [MaxLength(250, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string SupplierName { get; set; } = null!;
      

        [Display(Name = "Address")]
        [MaxLength(200, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Address { get; set; } = null!;
       
        [Display(Name = "Phone")]
        [MaxLength(20, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Phone { get; set; } = null!;

       

        public ICollection<Purchase>? Purchases { get; set; }
    }
}
