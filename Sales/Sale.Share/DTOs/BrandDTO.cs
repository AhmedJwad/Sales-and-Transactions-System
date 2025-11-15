using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class BrandDTO
    {
        public int Id { get; set; }
        public int SubcategoryId { get; set; }
        public List<BrandTranslationDTO>? brandTranslations { get; set; }
    }
}
