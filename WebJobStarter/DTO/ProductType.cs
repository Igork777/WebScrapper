using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebJobStarter.DTO
{
    public class ProductType
    {
        [Key]
        public int ProductTypeId { get; set; }

        public string Type { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}