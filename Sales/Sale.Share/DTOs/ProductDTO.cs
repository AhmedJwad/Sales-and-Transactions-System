using Sale.Share.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Name { get; set; } = null!;

        [Display(Name = "Barcode")]
        [MaxLength(20, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Barcode { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        [MaxLength(500, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Price")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Cost")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Cost { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Display(Name = "% Desired Profit")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal DesiredProfit { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Stock { get; set; }
        public int BrandId { get; set; }

        public bool HasSerial { get; set; }
        public List<int>? ProductCategoryIds { get; set; }
        public List<int>? ProductColorIds { get; set; }
        public List<int>? ProductSizeIds { get; set; }
        public List<string>? ProductImages { get; set; }
        public List<string>? SerialNumbers { get; set; }
    }
}
