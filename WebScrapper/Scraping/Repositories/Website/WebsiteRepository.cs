using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebScrapper.Scraping.DTO;

namespace WebScrapper.Scraping.ScrappingFluggerDk.Repositories
{
    public class WebsiteRepository: Repository<Website>, IWebsiteRepository
    {
        public WebsiteRepository(DbContext context) : base(context)
        {
        }
    }
}