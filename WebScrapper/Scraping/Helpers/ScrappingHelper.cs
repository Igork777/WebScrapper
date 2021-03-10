using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;

namespace WebScrapper.Scraping.Helpers
{
    public class ScrappingHelper
    {
        public static List<ProductType> _allProductTypes = new List<ProductType>()
        {
            new ProductType() {ProductTypeId = 1, Type = "Indoor", Products = new List<Product>()},
            new ProductType() {ProductTypeId = 2, Type = "Outdoor", Products = new List<Product>()},
            new ProductType() {ProductTypeId = 3, Type = "Tool", Products = new List<Product>()},
            new ProductType() {ProductTypeId = 4, Type = "Other", Products = new List<Product>()}
        };

        public static List<String> proxies;
        public static List<int> ports;

        public static List<Website> _allWebsites = new List<Website>()
        {
            new() {WebsiteId = 1, Name = "flugger.dk", Products = new List<Product>()},
            new Website() {WebsiteId = 2, Name = "flugger-helsingor.dk", Products = new List<Product>()},
            new Website() {WebsiteId = 3, Name = "www.maling-halvpris.dk", Products = new List<Product>()},
            new Website() {WebsiteId = 4, Name = "www.flugger-horsens.dk", Products = new List<Product>()}
        };
        public static readonly Regex InvalidCharacter = new Regex(@"&#[0-9]+[;]|&[A-Za-z]+[;]");
        public static readonly Regex IpRegex = new Regex(@"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\:[0-9]{1,5}\b");
        public static HtmlDocument GetHtmlDocument(String url)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDocument = web.Load(url);
            return htmlDocument;
        }
        
        public static void RenewIpAndPorts()
        {
            Dictionary<String, int> proxiesAndPorts = GetProxyAndPort();
            proxies = proxiesAndPorts.Keys.ToList();
            ports = proxiesAndPorts.Values.ToList();
        }

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
            String hash = hashData(product.Name + product.Size + product.ProductTypeId + product.WebsiteId +
                                   product.PathToImage);
            product.Hash = hash;
            Product similarProduct = ExistsAlreadyInTheDatabase(dbContext, hash);
            if (similarProduct != null)
            {
                if (!similarProduct.Price.Equals(product.Price))
                {
                    similarProduct.Price = product.Price;
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

        private static String hashData(String content)
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
                .Replace("Æ", "Ae").Replace("æ", "ae").
                Replace("Ø","O").Replace("ø", "o").
                Replace("Å","A").Replace("å", "a")
                .Replace("ß", "B");
        }
        
      
        
        public static HtmlDocument GetHtmlDocument(String url, String proxy, int port)
        {
            WebProxy prox = new WebProxy(proxy, port);
            prox.UseDefaultCredentials = true;
            prox.Credentials = CredentialCache.DefaultCredentials;
          
            WebClient client = new WebClient();
            client.Proxy = prox;

            String baseHtml = "";
            byte[] pageContent = client.DownloadData(url);
            UTF8Encoding utf = new UTF8Encoding();
            baseHtml = utf.GetString(pageContent);
            HtmlDocument pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(baseHtml);

            return pageHtml;
        }
        public static Dictionary<String, int> GetProxyAndPort()
        {
            Dictionary<String, int> proxyAndPort = new Dictionary<String, int>();
            HtmlDocument htmlDocument = GetHtmlDocument("https://free-proxy-list.net/uk-proxy.html");
            HtmlNodeCollection ips = htmlDocument.DocumentNode.SelectNodes("//tbody/tr/td[1]");
            HtmlNodeCollection ports = htmlDocument.DocumentNode.SelectNodes("//tbody/tr/td[2]");
            Console.WriteLine();
            for (int i = 0; i < ips.Count; i++)
            {
               proxyAndPort.Add(ips[i].InnerText, Int32.Parse(ports[i].InnerText));
            }

            return proxyAndPort;


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