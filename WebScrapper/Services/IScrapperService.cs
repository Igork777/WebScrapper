using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebScrapper.Scraping.DTO;

namespace WebScrapper.Services
{
    public interface IScrapperService
    { 
        public IList<Suggestion> GetAllSuggestions(String name);
        public Dictionary<string, IList<Product>> GetAllProducts(string name);
    }
}