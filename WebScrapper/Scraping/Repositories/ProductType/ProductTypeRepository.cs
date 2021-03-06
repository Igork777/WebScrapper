using Microsoft.EntityFrameworkCore;
using WebScrapper.Scraping.DTO;

namespace WebScrapper.Scraping.ScrappingFluggerDk.Repositories
{
    public class ProductTypeRepository : Repository<ProductType>, IProductType
    {
        private IProductRepository _productRepositoryImplementation;

        public ProductTypeRepository(DbContext context) : base(context)
        {
        }
    }
}