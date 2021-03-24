using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RestSharp;
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

        public static List<Product> allProducts;
        public static List<Product> productsAddedDuringThisSession = new List<Product>();
        public static List<String> proxies;
        public static List<int> ports;
        public static int proxiesCounter = 0;

        public static List<Website> _allWebsites = new List<Website>()
        {
            new Website() {WebsiteId = 1, Name = "www.flugger.dk", Products = new List<Product>()},
            new Website() {WebsiteId = 2, Name = "www.flugger-helsingor.dk", Products = new List<Product>()},
            new Website() {WebsiteId = 3, Name = "www.maling-halvpris.dk", Products = new List<Product>()},
            new Website() {WebsiteId = 4, Name = "www.flugger-horsens.dk", Products = new List<Product>()}
        };

        public static readonly Regex InvalidCharacter = new Regex(@"&#[0-9]+[;]|&[A-Za-z]+[;]");

        public static readonly Regex IpRegex =
            new Regex(
                @"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\:[0-9]{1,5}\b");

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

        public static Dictionary<string, string> getIPAndPort()
        {
            Dictionary<String, String> proxy_port = new Dictionary<string, string>();
            // List<String> allowedCodes = new List<string>()
            // {
            //     "BE", "BG", "CZ", "DK", "DE", "EE", "IE", "GR", "ES", "FR", "HR", "IT", "CY", "LV", "LT", "LU", "HU",
            //     "MT", "NL", "AT", "PL", "PT", "RO", "SI", "SK", "FI", "SE", "IS", "LI", "NO", "CH", "GB"
            // };
            Regex ipFromJson = new Regex("\"ip\"(.*?),");
            Regex portFromJson = new Regex("\"port\"(.*?),");

            // List<String> givenCountries = new List<string>();


            //  Regex countryJson = new Regex("\"")
            Regex ip_port_Values = new Regex("[1-9]([0-9]+|\\.)+");

          
            var client = new RestClient("https://proxy-orbit1.p.rapidapi.com/v1/");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-key", "1959d36928msh662f443ae286ccep1b6f5ajsnadb0d5161bee");
            request.AddHeader("x-rapidapi-host", "proxy-orbit1.p.rapidapi.com");
            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);
            String rawProxy = ipFromJson.Match(response.Content).Value;
            String rawPort = portFromJson.Match(response.Content).Value;
            String proxy = ip_port_Values.Match(rawProxy).Value;
            String port = ip_port_Values.Match(rawPort).Value;
            Console.WriteLine(proxy+":"+port);
            return new Dictionary<string, string>();
        }

        public static KeyValuePair<String, String> getFreshIPAndPort()
        {
            
            Dictionary<String, String> proxy_port = new Dictionary<string, string>();
            List<String> allowedCodes = new List<string>()
            {
                "BE", "BG", "CZ", "DK", "DE", "EE", "IE", "GR", "ES", "FR", "HR", "IT", "CY", "LV", "LT", "LU", "HU",
                "MT", "NL", "AT", "PL", "PT", "RO", "SI", "SK", "FI", "SE", "IS", "LI", "NO", "CH", "GB"
            };
            Regex ipFromJson = new Regex("\"ip\"(.*?),");
            Regex portFromJson = new Regex("\"port\"(.*?),");

            // List<String> givenCountries = new List<string>();


            //  Regex countryJson = new Regex("\"")
            Regex ip_port_Values = new Regex("[1-9]([0-9]+|\\.)+");

            if (proxiesCounter == allowedCodes.Count)
            {
                proxiesCounter = 0;
            }

            IRestResponse response;
            do
            {
                var client = new RestClient("https://proxy-orbit1.p.rapidapi.com/v1/?location="+allowedCodes[proxiesCounter]+"&protocols=socks4");
                proxiesCounter++;
                var request = new RestRequest(Method.GET);
                request.AddHeader("x-rapidapi-key", "1959d36928msh662f443ae286ccep1b6f5ajsnadb0d5161bee");
                request.AddHeader("x-rapidapi-host", "proxy-orbit1.p.rapidapi.com");
                 response = client.Execute(request);
                 Console.WriteLine(response.Content);
            } while (!response.IsSuccessful);
            
            

           
            String rawProxy = ipFromJson.Match(response.Content).Value;
            String rawPort = portFromJson.Match(response.Content).Value;
            String proxy = ip_port_Values.Match(rawProxy).Value;
            String port = ip_port_Values.Match(rawPort).Value;
            KeyValuePair<String, String> proxyAndPort = new KeyValuePair<string, string>(proxy,port);
            return proxyAndPort;
        }
        public static ChromeOptions setProxyOption(String prox, String port)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            Proxy proxy = new Proxy();
            proxy.Kind = ProxyKind.Manual;
            proxy.IsAutoDetect = false;
            proxy.HttpProxy =
                proxy.SslProxy = prox + ":" + port;
            chromeOptions.Proxy = proxy;
            chromeOptions.AddArgument("ignore-certificate-errors");
            return chromeOptions;
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
                product.Name = product.Name.Trim();
                productsAddedDuringThisSession.Add(product);
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
        public static void removeDeletedProductsFromDB(DBContext dbContext)
        {
            var a = allProducts;
            var b = productsAddedDuringThisSession;
            bool isFound = false;
            for (int i = 0; i < allProducts.Count; i++)
            {
                for (int j = 0; j < productsAddedDuringThisSession.Count; j++)
                {
                    if (allProducts[i].Hash.Equals(productsAddedDuringThisSession[j].Hash))
                    {
                        Console.WriteLine(allProducts[i].Name + "corresponds");
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                {
                    Console.WriteLine(allProducts[i].Name + "doesn't correspond");
                    dbContext.Product.Remove(allProducts[i]);
                    dbContext.SaveChanges();
                    Console.WriteLine(dbContext.Product.ToList().Count);
                }
                isFound = false;
            }
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
            Dictionary<String, int> proxyAndPort = new Dictionary<string, int>();
            string[] lines = File.ReadAllLines("Scraping\\ip.txt");

            foreach (string line in lines)
            {
                List<String> prox_port = line.Split(":").ToList();
                try
                {
                    proxyAndPort.Add(prox_port[0], Int32.Parse(prox_port[1]));
                }
                catch (Exception e)
                {
                }
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