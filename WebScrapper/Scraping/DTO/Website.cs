using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebScrapper.Scraping.DTO
{
    public class Website
    {
        [Key]
        public int WebsiteId { get; set; }
        public string Name { get; set; }
    }
}