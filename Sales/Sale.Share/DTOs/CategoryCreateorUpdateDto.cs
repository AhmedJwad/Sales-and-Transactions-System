using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class CategoryCreateorUpdateDto
    {
        public int Id { get; set; }
        public string? Photo { get; set; } // base64 string optional
        public List<CategoryTranslationDto> Translations { get; set; } = new();
    }
}
