using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class SizeDTO
    {
        public int Id { get; set; }        
        public string Name { get; set; } = null!;
    }
}
