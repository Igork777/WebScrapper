using Microsoft.EntityFrameworkCore;
using WebScrapper.Scraping.DTO;

namespace WebScrapper.Scraping.ScrappingFluggerDk.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(DbContext context) : base(context)
        {
            
        }
        
    }
    
    
}