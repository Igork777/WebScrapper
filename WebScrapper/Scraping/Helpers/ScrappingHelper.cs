using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;

namespace WebScrapper.Scraping.Helpers
{
    public class ScrappingHelper
    {
        public static List<ProductType> _allProductTypes = new List<ProductType>()
        {
            new() { Type = "Indoor", Products = new List<Product>()},
            new() { Type = "Outdoor", Products = new List<Product>()},
            new() { Type = "Other", Products = new List<Product>()}
        };

        public static List<Product> allProducts;
        public static List<Product> productsAddedDuringThisSession = new List<Product>();

        public static List<Website> _allWebsites = new List<Website>()
        {
            new() { Name = "www.flugger.dk", Products = new List<Product>()},
            new() { Name = "www.flugger-helsingor.dk", Products = new List<Product>()},
            new() { Name = "www.maling-halvpris.dk", Products = new List<Product>()},
            new() { Name = "www.flugger-horsens.dk", Products = new List<Product>()},
            new() { Name = "www.flugger-naerum.dk", Products = new List<Product>()}
        };

        public static readonly Regex InvalidCharacter = new Regex(@"&#[0-9]+[;]|&[A-Za-z]+[;]");


        private static Product ExistsAlreadyInTheDatabase(DBContext dbContext, String hash)
        {
            Product product = dbContext.Product.FirstOrDefault(node => node.Hash.Equals(hash));
            return product;
        }


        private static void AddToProductType(DBContext _dbContext, Product product)
        {
            ProductType productType =
                _dbContext.ProductType.FirstOrDefault(node =>
                    node.ProductTypeId == product.ProductTypeId);
            if (productType == null)
            {
                throw new RuntimeWrappedException("The product type must be added");
            }

            if (productType.Products == null)
            {
                productType.Products = new List<Product>();
            }

            productType.Products.Add(product);
            _dbContext.SaveChanges();
        }

        private static void AddToWebsite(DBContext _dbContext, Product product)
        {
            Website website = _dbContext.Website.FirstOrDefault(node => node.WebsiteId == product.WebsiteId);
            if (website == null)
            {
                throw new RuntimeWrappedException("The website id must be added");
            }

            if (website.Products == null)
            {
                website.Products = new List<Product>();
            }

            website.Products.Add(product);
            _dbContext.SaveChanges();
        }


        public static void SaveOrUpdate(DBContext dbContext, Product product)
        {
            String hash = hashData(product.Name + product.Size + product.WebsiteId);
            product.Hash = hash;
            product.Name = product.Name.Trim();
            product.UpdatedAt = DateTime.Now;
            productsAddedDuringThisSession.Add(product);
            Product similarProduct = ExistsAlreadyInTheDatabase(dbContext, hash);
            if (similarProduct != null)
            {
                Console.WriteLine(similarProduct.CurrentPrice);
                Console.WriteLine(product.CurrentPrice);
                if (Int32.Parse(similarProduct.CurrentPrice) != Int32.Parse(product.CurrentPrice))
                {
                    similarProduct.OldPrice = similarProduct.CurrentPrice.Replace(",", ".").Replace(" ", "")
                        .Replace("L", "").Replace(".", "");
                    similarProduct.CurrentPrice = product.CurrentPrice.Replace(",", ".").Replace(" ", "")
                        .Replace("L", "").Replace(".", "");
                    similarProduct.UpdatedAt = DateTime.Now;
                    dbContext.SaveChanges();
                }
            }
            else
            {
                dbContext.Product.Add(product);
                dbContext.SaveChanges();
                AddToProductType(dbContext, product);
                AddToWebsite(dbContext, product);
            }
        }

        public static String hashData(String content)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(content));
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }

            return hash.ToString();
        }

        public static string RemoveDiacritics(string text)
        {
            return text.Replace("Ü", "U").Replace("ü", "u")
                .Replace("Æ", "Ae").Replace("æ", "ae").Replace("Ø", "O").Replace("ø", "o").Replace("Å", "A")
                .Replace("å", "a").Replace("ä", "a").Replace("Ä", "A")
                .Replace("ß", "B");
        }


        public static void LoadAllProducts(DBContext dbContext)
        {
            allProducts = dbContext.Product.ToList();
        }

        [SuppressMessage("ReSharper.DPA", "DPA0004: Closure object allocation")]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void removeDeletedProductsFromDB(DBContext dbContext)
        {
            var a = allProducts;
            var b = productsAddedDuringThisSession;
            bool isFound = false;
            for (int i = 0; i < allProducts.Count; i++)
            {
                Product correspondentProduct =
                    productsAddedDuringThisSession.FirstOrDefault(node => node.Hash.Equals(allProducts[i].Hash));
                if (correspondentProduct == null)
                {
                    Console.WriteLine(allProducts[i] + " was deleted");
                    dbContext.Product.Remove(allProducts[i]);
                }
            }
        }

        public static String FixInvalidCharacter(String name, Regex invalidCharacter)
        {
            String fixedName = name;
            MatchCollection incorrectCharacters = invalidCharacter.Matches(name);
            foreach (Match incorrectCharacter in incorrectCharacters)
            {
                fixedName = fixedName.Replace(incorrectCharacter.Value,
                    HttpUtility.HtmlDecode(incorrectCharacter.Value));
            }

            return fixedName;
        }


        public static bool CheckIfInvalidCharacter(String name, Regex regex)
        {
            return regex.IsMatch(name);
        }
    }
}