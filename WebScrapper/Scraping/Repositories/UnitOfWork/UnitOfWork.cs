using Microsoft.EntityFrameworkCore;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;

namespace WebScrapper.Scraping.ScrappingFluggerDk.Repositories
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly DBContext _context;
        public UnitOfWork(DBContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
            ProductsType = new ProductTypeRepository(_context);
            Website = new WebsiteRepository(_context);

        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IProductRepository Products { get; }
        public ProductTypeRepository ProductsType { get; }
        public IWebsiteRepository Website { get; }

        public int Complete()
        {
           return _context.SaveChanges();
        }
    }
}