using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebJobStarter.DbContext;
using WebJobStarter.DTO;
using WebJobStarter.Enums;
using WebJobStarter.Exceptions;

namespace WebJobStarter.Helpers
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

        public static ChromeOptions getOptions()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--disable-background-timer-throttling");
            options.AddArgument("--disable-backgrounding-occluded-windows");
            options.AddArgument("--disable-breakpad");
            options.AddArgument("--disable-component-extensions-with-background-pages");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-features=TranslateUI,BlinkGenPropertyTrees");
            options.AddArgument("--disable-ipc-flooding-protection");
            options.AddArgument("--disable-renderer-backgrounding");
            options.AddArgument("--enable-features=NetworkService,NetworkServiceInProcess");
            options.AddArgument("--force-color-profile=srgb");
            options.AddArgument("--hide-scrollbars");
            options.AddArgument("--metrics-recording-only");
            options.AddArgument("--mute-audio");
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
         //  options.AddArgument("--disable-dev-shm-usage");
            options.AddAdditionalCapability("browserless.token", "b7fd89cf-242c-49c8-999b-8cb41e528c68", true);
            return options;

        }

        //this method returns an element from the drive
        public static IWebElement GetElementWhenVisible(WebDriverWait wait, TypeOfAttribute typeOfAttribute, String valueOfAttribute)
        {
            IWebElement element = null;
            if (typeOfAttribute.Equals(TypeOfAttribute.ClassAttr))
            {
                element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName(valueOfAttribute)));
            }
            else if (typeOfAttribute.Equals(TypeOfAttribute.IdAttr))
            {
                element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(valueOfAttribute)));
            }
            else if (typeOfAttribute.Equals(TypeOfAttribute.TagAttr))
            { 
                element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.TagName(valueOfAttribute)));
            }

            if (element == null)
            {
                throw new ElementDoesntExistException();
            }
            return element;
        }

        //this method returns an element from specific IWebElement
        public static IWebElement GetElementWhenVisible(WebDriverWait wait, TypeOfAttribute typeOfAttribute, String valueOfAttribute, IWebElement elementToGetAnElement)
        {
            IWebElement element = null;
            try
            {
                GetElementWhenVisible(wait, typeOfAttribute, valueOfAttribute);
                if (typeOfAttribute.Equals(TypeOfAttribute.ClassAttr))
                {
                    element = elementToGetAnElement.FindElement(By.ClassName(valueOfAttribute));
                }
                else if (typeOfAttribute.Equals(TypeOfAttribute.IdAttr))
                {
                    element = elementToGetAnElement.FindElement(By.Id(valueOfAttribute));
                }
                else if (typeOfAttribute.Equals(TypeOfAttribute.TagAttr))
                {
                  element = elementToGetAnElement.FindElement(By.TagName(valueOfAttribute));
                }
            }
            catch (ElementDoesntExistException)
            {
                throw;
            }
            return element;
        }


        //this element return a list of elements from specific IWebElement
        public static List<IWebElement> GetElementsWhenVisible(WebDriverWait wait, TypeOfAttribute typeOfAttribute, String valueOfAttribute, IWebElement elementToGetListOfElements) 
        {
            try
            {
                GetElementWhenVisible(wait, typeOfAttribute, valueOfAttribute);
                if (typeOfAttribute.Equals(TypeOfAttribute.ClassAttr))
                {
                    return elementToGetListOfElements.FindElements(By.ClassName(valueOfAttribute)).ToList();
                }
                else if (typeOfAttribute.Equals(TypeOfAttribute.TagAttr))
                {
                    return elementToGetListOfElements.FindElements(By.TagName(valueOfAttribute)).ToList();
                }

                throw new RuntimeWrappedException("Incorrect Attribute");
                
            }
            catch (ElementDoesntExistException)
            {
                throw;
            }
            
        }

    }
}