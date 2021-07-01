using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public IEnumerable<ComparedProduct> GetAllProductsThatAreWorseThenFluggers()
        {
            IList<ComparedProduct> comparedProducts = new List<ComparedProduct>();
            List<Product> retirevedProducts = new List<Product>();
            
           List<Product> allFluggerProducts = _dbContext.Product.Where(node => node.WebsiteId == 4).ToList();
           foreach (Product product in allFluggerProducts)
           {
               retirevedProducts.AddRange(_dbContext.Product.Where(node => node.Name.Equals(product.Name) && node.Size.Equals(product.Size) && node.WebsiteId != 4));
               foreach (Product p in retirevedProducts)
               {
                   ComparedProduct comparedProduct = getComparedProduct(p);
                   if(comparedProduct == null || comparedProduct.lowerPrices.Count == 0)
                       continue;
                   comparedProducts.Add(comparedProduct);
               }
           }

           return comparedProducts;
        }


        public IList<Product> GetLatestPriceUpdatedProduct()
        {
            List<Product> latestUpdatedProducts = _dbContext.Product
                .Where(p => p.WebsiteId != 4 && p.UpdatedAt > DateTime.Now.AddDays(-30)).ToList();
            return latestUpdatedProducts;
        }

        private ComparedProduct getComparedProduct (Product product)
        {
            ComparedProduct comparedProduct = new ComparedProduct();
            List<Product> products = _dbContext.Product.Where(node => node.Name.Equals(product.Name) && node.Size.Equals(product.Size) && node.WebsiteId != 4).ToList();
            if (products.Count == 0)
            {
                return null;
            }

            if (products.Count > 3)
            {
                throw new RuntimeWrappedException("WTF Igor");
            }
            
            for (int i = 0; i < products.Count; i++)
            {
                if (Double.Parse(products[i].CurrentPrice) < Double.Parse(product.CurrentPrice))
                {
                    comparedProduct.lowerPrices.Add(products[i]);
                }
                else
                {
                    comparedProduct.higherOrSamePrice.Add(products[i]);
                }
            }

            comparedProduct.fluggerProduct = product;
            return comparedProduct;
        }
        
        
    }
    
}