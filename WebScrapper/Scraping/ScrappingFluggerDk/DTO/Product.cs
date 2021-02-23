using System;
using System.Collections.Generic;

namespace WebScrapper.Scraping.DTO
{
    public class Product
    {
        public string Name { get; set; }
        public Dictionary<String, String> SizeToPrice = new Dictionary<String, String>();
        
    }
}