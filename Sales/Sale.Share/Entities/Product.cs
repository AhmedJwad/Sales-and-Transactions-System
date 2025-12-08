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

        [Display(Name = "Barcode")]
        [MaxLength(20, ErrorMessage = "Field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public string Barcode { get; set; } = null!;          
        public decimal Stock { get; set; }
        public bool HasSerial { get; set; }
        public DateTime CreatedAt { get; set; }
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
        public ICollection<ProductColor>? productColor { get; set; }
        public ICollection<productSize>? productSize { get; set; }
        public ICollection<ProductDiscount>? productDiscount { get; set; }
        public ICollection<Rating>? rating { get; set; }
        public ICollection<OrderDetail>? orderDetail { get; set; }
        public ICollection<ProductTranslation>? ProductTranslations { get; set; }
        public ICollection<ProductPrice>? ProductPrices { get; set; }
    }
 }
