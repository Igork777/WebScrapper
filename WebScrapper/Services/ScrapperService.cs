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
            IList<Product> products = _dbContext.Product
                .Where(product => product.Name.ToLower().Contains(name.ToLower())).ToList();


            foreach (Product product in products)
            {
                names.Add(product.Name);
            }

            foreach (String n in names)
            {
                suggestions.Add(new Suggestion
                    {label = n, value = ScrappingHelper.RemoveDiacritics(n.Replace(" ", "-").ToLower())});
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
                retirevedProducts.Clear();
                retirevedProducts.AddRange(_dbContext.Product.Where(node =>
                    node.Name.Equals(product.Name) && node.Size.Equals(product.Size) && node.WebsiteId != 4));

                ComparedProduct comparedProduct = getComparedProduct(product, retirevedProducts);
                if (comparedProduct == null)
                    continue;
                
                comparedProducts.Add(comparedProduct);
            }

            return comparedProducts;
        }


        public IList<ProductChangedPrice> GetLatestPriceUpdatedProduct()
        {
            IList<ProductChangedPrice> productChangedPrices = new List<ProductChangedPrice>();
            List<Product> latestUpdatedProducts = _dbContext.Product
                .Where(p => p.WebsiteId != 4 && p.OldPrice != null && p.UpdatedAt > DateTime.Now.AddDays(-30)).OrderByDescending(p => p.UpdatedAt).ToList();
            for (int i = 0; i < latestUpdatedProducts.Count; i++)
            {
                ProductChangedPrice productChangedPrice = new ProductChangedPrice();
                Product currentProduct = latestUpdatedProducts[i];
                productChangedPrice.productId = currentProduct.ProductId;
                productChangedPrice.name = currentProduct.Name;
                productChangedPrice.date = currentProduct.UpdatedAt;
                productChangedPrice.size = currentProduct.Size;
                productChangedPrice.currentPrice = currentProduct.CurrentPrice;
                productChangedPrice.oldPrice = currentProduct.OldPrice;
                productChangedPrice.productUrl = currentProduct.PathToImage;
                if (currentProduct.WebsiteId == 1)
                {
                    productChangedPrice.website = "www.flugger.dk";
                }
                else if (currentProduct.WebsiteId == 2)
                {
                    productChangedPrice.website = "www.flugger-helsingor.dk";
                }
                else if (currentProduct.WebsiteId == 3)
                {
                    productChangedPrice.website = "www.maling-halvpris.dk";
                }
                productChangedPrices.Add(productChangedPrice);
            }
            return productChangedPrices;
        }

        private ComparedProduct getComparedProduct(Product productFromFluggerHorsens, List<Product> productsToCompare)
        {
            List<Product> smallerOrTheSame = new List<Product>();
            ComparedProduct comparedProduct = null;
           
            if (productsToCompare.Count == 0)
            {
                return comparedProduct;
            }

            if (productsToCompare.Count > 3)
            {
                throw new RuntimeWrappedException("WTF Igor");
            }

            for (int i = 0; i < productsToCompare.Count; i++)
            {
                if (Double.Parse(productsToCompare[i].CurrentPrice) < Double.Parse(productFromFluggerHorsens.CurrentPrice))
                {
                    smallerOrTheSame.Add(productsToCompare[i]);
                }
            }

            if (smallerOrTheSame.Count > 0)
            {
                ProductLowerPriceComparison fluggerHorsensProduct  = InitializeProductLowerPriceComparison(productFromFluggerHorsens);
                comparedProduct = new ComparedProduct();
                comparedProduct.fluggerHorsens = fluggerHorsensProduct;
                for (int q = 0; q < productsToCompare.Count; q++)
                {
                    if (productsToCompare[q].WebsiteId == 1)
                    {
                        comparedProduct.flugger =InitializeProductLowerPriceComparison(productsToCompare[q]);
                    }

                    else if (productsToCompare[q].WebsiteId == 2)
                    {
                        comparedProduct.fluggerHelsingor = InitializeProductLowerPriceComparison(productsToCompare[q]);
                    }
                    else
                    {
                        comparedProduct.malingHalvpris = InitializeProductLowerPriceComparison(productsToCompare[q]);
                    }


                }
            }
            return comparedProduct;
        }

        private ProductLowerPriceComparison InitializeProductLowerPriceComparison(Product product)
        {
            ProductLowerPriceComparison fluggerProduct = new ProductLowerPriceComparison();
            fluggerProduct.productId = product.ProductId;
            fluggerProduct.productUrl = product.PathToImage;
            fluggerProduct.name = product.Name;
            fluggerProduct.size = product.Size;
            fluggerProduct.currentPrice = product.CurrentPrice;
            if (product.WebsiteId == 1)
            {
                fluggerProduct.shopName = "Flugger DK";
                fluggerProduct.website = "flugger.dk";
            }
            if (product.WebsiteId == 2)
            {
                fluggerProduct.shopName = "Flugger Helsingor";
                fluggerProduct.website = "flugger-helsingor.dk";
            }
            else if (product.WebsiteId == 3)
            {
                fluggerProduct.shopName = "Maling Halvpris";
                fluggerProduct.website = "maling-halvpris.dk";
            }
            else
            {
                fluggerProduct.shopName = "Flugger Horsens";
                fluggerProduct.website = "flugger-horsens.dk";
            }

            return fluggerProduct;
        }
    }
}