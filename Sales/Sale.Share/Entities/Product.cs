using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class Product
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

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Profit")]
        public decimal Profit => Price - Cost;

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Value of Cost")]
        public decimal CostValue => Cost * Stock;

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Value of Price")]
        public decimal PriceValue => Price * Stock;

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Display(Name = "% Pure Profit")]
        public decimal RealProfit => Cost == 0 ? 0 : Profit / Cost;

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [Display(Name = "% Desired Profit")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal DesiredProfit { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public decimal Stock { get; set; }        
        public bool HasSerial { get; set; }
        public ICollection<ProductsubCategory>? productsubCategories { get; set; }
        public int BrandId { get; set; }
        public Brand? brand { get; set; }
        [Display(Name = "Categories")]
        public int ProductCategoriesNumber => productsubCategories?.Count ?? 0;

        public ICollection<ProductImage>? ProductImages { get; set; }

        [Display(Name = "Images")]
        public int ProductImagesNumber => ProductImages?.Count ?? 0;

        [Display(Name = "Main Image")]
        public string MainImage => ProductImages?.FirstOrDefault()?.Image ?? string.Empty;

        public ICollection<SerialNumber>? serialNumbers { get; set; }
    }
}
