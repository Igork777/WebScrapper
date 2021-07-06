using System;

namespace WebScrapper.Scraping.DTO
{
    public class ProductChangedPrice
    {
        public int productId { get; set; }
        public String oldPrice { get; set; }
        public String currentPrice { get; set; }
        public String name { get; set; }
        public String size { get; set; }
        public String productUrl { get; set; }
        public String website { get; set; }
        public DateTime date { get; set; }
    }
}