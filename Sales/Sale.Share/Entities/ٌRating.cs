using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class Rating
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(2,1)")]
        [Range(1, 5, ErrorMessage = "Stars must be between 1.0 and 5.0")]
        public decimal Stars { get; set; } 

        [MaxLength(500)]
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;      
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
