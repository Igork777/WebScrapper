using System.Collections;
using System.Collections.Generic;

namespace WebScrapper.Scraping.DTO
{
    public class ComparedProduct
    {
        public Product fluggerProduct { get; set; }
        public IList<Product> lowerPrices = new List<Product>();
        public IList<Product> higherOrSamePrice = new List<Product>();

        public IList<Product> LowerPrices
        {
            get => lowerPrices;
            set => lowerPrices = value;
        }

        public IList<Product> HigherOrSamePrice
        {
            get => higherOrSamePrice;
            set => higherOrSamePrice = value;
        }
    }
}