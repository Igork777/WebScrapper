using System;
using System.Collections.Generic;

namespace WebScrapper.Scraping.DTO
{
    public class Product
    {
        public string Name { get; set; }
        public Dictionary<float, int> SizeToPrice = new Dictionary<float, int>();
        
    }
}