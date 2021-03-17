using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;

namespace WebScrapper.Services
{
    public class ScrapperService : IScrapperService
    {
        private DBContext _dbContext;
        public ScrapperService(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<Suggestion> GetAllSuggestions(string name)
        {
            HashSet<String> names = new HashSet<string>();
            IList<Suggestion> suggestions = new List<Suggestion>();
            IList<Product> products = _dbContext.Product.Where(product => product.Name.ToLower().Contains(name.ToLower())).ToList();
            
            
            foreach (Product product in products)
            {
                names.Add(product.Name);
            }

            foreach (String n in names)
            {
                suggestions.Add(new Suggestion {label = n, value = ScrappingHelper.RemoveDiacritics(n.Replace(" ", "-").ToLower())});
            }
            return suggestions;
        }

        public Dictionary<string, IList<Product>> GetAllProducts(string name)
        {
            Dictionary<string, IList<Product>> listOfProducts = new Dictionary<string, IList<Product>>();
            List<Product> products = _dbContext.Product.Where(node => node.Name.Equals(name)).ToList();

            listOfProducts.Add("flugger.dk", products.Where(node => node.WebsiteId == 1).ToList());
            listOfProducts.Add("flugger-helsingor.dk", products.Where(node => node.WebsiteId == 2).ToList());
            listOfProducts.Add("www.maling-halvpris.dk", products.Where(node => node.WebsiteId == 3).ToList());
            listOfProducts.Add("www.flugger-horsens.dk", products.Where(node => node.WebsiteId == 4).ToList());
            
            return listOfProducts;
        }
    }
    
}