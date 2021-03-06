using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebScrapper.Scraping.DTO
{
    public class ProductType
    {
        [Key]
        public int ProductTypeId { get; set; }

        public string Type { get; set; }
    }
}