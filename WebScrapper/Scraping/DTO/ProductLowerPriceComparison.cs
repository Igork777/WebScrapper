namespace WebScrapper.Scraping.DTO
{
    public class ProductLowerPriceComparison
    {
        public int productId { get; set; }
        public string productUrl { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string currentPrice { get; set; }
        public string shopName { get; set; }
        public string website { get; set; }
    }
}