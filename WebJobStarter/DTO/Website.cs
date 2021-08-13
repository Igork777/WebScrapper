using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebJobStarter.DTO
{
    public class Website
    {
        [Key]
        public int WebsiteId { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}