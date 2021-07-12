using System.Collections;
using System.Collections.Generic;

namespace WebScrapper.Scraping.DTO
{
    public class ComparedProduct
    {
        public ProductLowerPriceComparison fluggerHorsens { get; set; }

        public ProductLowerPriceComparison malingHalvpris { get; set; }

        public ProductLowerPriceComparison flugger { get; set; }

        public ProductLowerPriceComparison fluggerHelsingor { get; set; }
        public ProductLowerPriceComparison fluggerNaerum { get; set; }
    }
}