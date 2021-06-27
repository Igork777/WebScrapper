using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebScrapper.Scraping.DTO
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        
        public string Hash { get; set; }
        
        public string Name { get; set; }
        public string Size { get; set; }
        public string Price { get; set; }
        
        public string PathToImage { get; set; }
        public ProductType ProductType { get; set; }
        public int ProductTypeId { get; set; }
        public int WebsiteId { get; set; }
        
        public Website Website { get; set; }

        public override string ToString()
        {
            return "Name: " + Name + "\n" + "Size: " + Size + "\n" + "Price: " + Price + "\n" + "Path to image: " +
                   PathToImage;
        }
    }
}