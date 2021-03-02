using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebScrapper.Scraping.DTO
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        
        public string Name { get; set; }
        public string Size { get; set; }
        public string Price { get; set; }
        public ProductType ProductType { get; set; }

        public int WebsiteId { get; set; }
        public Website Website { get; set; }

    }
}