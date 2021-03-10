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

        public Dictionary<String, String> GetAllSuggestions(string name)
        {
            HashSet<String> names = new HashSet<string>();
            Dictionary<String, String> namesWithLabels = new Dictionary<string, string>();
            IList<Product> products = _dbContext.Product.Where(product => product.Name.ToLower().Contains(name.ToLower())).ToList();
            String latinName;
            foreach (Product product in products)
            {
                String nameWithoutDiacretese = ScrappingHelper.RemoveDiacritics(product.Name);
                String defisInsteadOfSpaces = nameWithoutDiacretese.Replace(" ", "-");
                Console.WriteLine(defisInsteadOfSpaces);
                names.Add(nameWithoutDiacretese);
            }

            foreach (String n in names)
            {
                namesWithLabels.Add(n, n.Replace(" ", "-").ToLower());
            }
            return namesWithLabels;
        }
    }
    
}