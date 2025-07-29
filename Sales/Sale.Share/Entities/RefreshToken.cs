using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }       
        public string? Token { get; set; } = null!;        
        public string? UserId { get; set; } = null!;
        public DateTime ExpirationDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsRevoked { get; set; } = false;        
        public User User { get; set; } = null!;
    }
}
