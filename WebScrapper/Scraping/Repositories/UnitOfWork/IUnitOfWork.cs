using System;

namespace WebScrapper.Scraping.ScrappingFluggerDk.Repositories
{
    public interface IUnitOfWork : IDisposable 
    {
        IProductRepository Products { get; }
        ProductTypeRepository ProductsType { get; }
        IWebsiteRepository Website { get; }
        int Complete();
    }
}