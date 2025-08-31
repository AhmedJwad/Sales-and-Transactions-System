using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class Discount
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100.")]
        [Display(Name = "Discount Percent")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal DiscountPercent { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime Endtime { get; set; }
        public bool isActive { get; set; }
        public ICollection<ProductDiscount>? productDiscounts { get; set; }
    }
}
