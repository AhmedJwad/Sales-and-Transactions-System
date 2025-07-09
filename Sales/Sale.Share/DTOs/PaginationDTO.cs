﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
   public class PaginationDTO
    {
        public int Id { get; set; }
        public int Page { get; set; } = 1;
        public int RecordsNumber { get; set; } =10;
        public string? Filter { get; set; }
        public string? CategoryFilter { get; set; }
        public int? CategoryId { get; set; }

    }
}
