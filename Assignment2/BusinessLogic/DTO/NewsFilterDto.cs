using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class NewsFilterDto
    {
        public string? Keyword { get; set; }
        public string? Category { get; set; }
        public string? DateField { get; set; } = "created"; // created / verified
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Sort { get; set; } = "created_desc"; // created_asc, title_desc, ...
    }
}
