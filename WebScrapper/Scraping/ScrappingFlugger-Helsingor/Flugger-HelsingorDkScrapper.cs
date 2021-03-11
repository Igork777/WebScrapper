using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Scraping.ScrappingFluggerDk.Enums;


namespace WebScrapper.Scraping
{
    public class FluggerHelsingorDkScrapper
    {
        private DBContext _dbContext;

        public FluggerHelsingorDkScrapper(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.String")]
        public void StartScrapping()
        {
            Start("https://flugger-helsingor.dk/vare-kategori/indendoers-maling/loft-og-vaeg/",
                TypesOfProduct.Indoors);

            Start(
                "https://flugger-helsingor.dk/vare-kategori/indendoers-maling/trae-og-metal/",
                TypesOfProduct.Indoors);
            Start(
                "https://flugger-helsingor.dk/vare-kategori/indendoers-maling/grundere-indendors-maling/",
                TypesOfProduct.Indoors);
            
            Start("https://flugger-helsingor.dk/vare-kategori/udendors-maling/facade/",
                TypesOfProduct.Outdoors);
            Start(
                "https://flugger-helsingor.dk/vare-kategori/udendors-maling/traebeskyttelse-og-metalmaling/",
                TypesOfProduct.Outdoors);
            Start(
                "https://flugger-helsingor.dk/vare-kategori/udendors-maling/grundere-udendors-maling/",
                TypesOfProduct.Outdoors);
            
            Start(
                "https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvlak-gulvbehandling/",
                TypesOfProduct.Others);
            Start("https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvmaling/",
                TypesOfProduct.Others);
            Start(
                "https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvolie-gulvbehandling/",
                TypesOfProduct.Others);
            Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/moebelpleje/",
                TypesOfProduct.Others);
            Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/linolier/",
                TypesOfProduct.Others);
            Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/bejdse/",
                TypesOfProduct.Others);
            
            Start("https://flugger-helsingor.dk/vare-kategori/tilbehor/rengoring-og-afdaekning/",
                TypesOfProduct.Tools);
            Start("https://flugger-helsingor.dk/vare-kategori/tilbehor/spartel-og-fuge/",
                TypesOfProduct.Tools);
            Start(
                "https://flugger-helsingor.dk/vare-kategori/tilbehor/vaegbeklaedning-og-klaebere/",
                TypesOfProduct.Tools);
            Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/bakker-og-spande/",
                TypesOfProduct.Tools);
            Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/malerruller/",
                TypesOfProduct.Tools);
            Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/pensler/",
                TypesOfProduct.Tools);
            Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/spartler-og-skrabere/",
                TypesOfProduct.Others);
            Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/vaegbeklaedning/",
                TypesOfProduct.Tools);

           
        }

        private void Start(String urlToScrap, Enum type)
        {
           
            List<String> pages = GetPages(urlToScrap);
            foreach (String page in pages)
            {
                try
                {
                    GetProducts(page, type);
                }
                catch (Exception e)
                {
                   continue;
                }
             
            }
        }

        private void GetProducts(string page, Enum type)
        {
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            driver.Navigate().GoToUrl(page);
            CleanWindows(driver);
            List<String> link = GetLinks(driver);
            for (int i = 0; i < link.Count; i++)
            {
                Product product = GetProduct(link[i]);
                Console.WriteLine("Name: "+ product.Name);
                Console.WriteLine("Path to image:" + product.PathToImage);
                Console.WriteLine("Size: " + product.Size);
                Console.WriteLine("Price: " + product.Price);
                product.ProductTypeId = Convert.ToInt32(type);
                product.WebsiteId = 2;
                ScrappingHelper.SaveOrUpdate(_dbContext, product);
            }
            driver.Quit();
        }

        private String GetImagePath(IWebDriver driver)
        {
            IWebElement pathToImage = driver.FindElement(By.ClassName("woocommerce-product-gallery__wrapper"));
            String href = "No data";
            try
            {
                IWebElement a = pathToImage.FindElement(By.TagName("a"));
                href = a.GetAttribute("href");
            }
            catch (Exception e)
            {
                Console.WriteLine("No link for image exists");
            } ;
            return href;
        }

        private Product GetProduct(String link)
        {
         
            Regex priceRegex = new Regex("[1-9]([0-9]{0,2}|\\.)+,");
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver(options);
            KeyValuePair<String, String> sizesAndPrices;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            driver.Navigate().GoToUrl(link);
            CleanWindows(driver);
            Product product = new Product();
            product.PathToImage = GetImagePath(driver);  
            IWebElement nameElement = driver.FindElement(By.ClassName("et_pb_module_inner"));
            product.Name = nameElement.Text; 

            Thread.Sleep(4000);
            IWebElement price = driver.FindElement(By.ClassName("price"));
            IWebElement pr = null;
            String priceString = "";
            try
            {
                 pr = price.FindElement(By.TagName("ins"));
                 priceString = priceRegex.Match(pr.Text).Value.Replace(",","");
                Console.WriteLine("Price: " + priceString);
            }
            catch (Exception e)
            {
                Console.WriteLine("No old price");
                
                priceString  = priceRegex.Match(price.Text).Value.Replace(",","");
                Console.WriteLine("Price: " + priceString);
            }

            try
            {
                IWebElement radio = driver.FindElement(By.Id("picker_pa_stoerrelse_ml"));
                IWebElement label = radio.FindElement(By.TagName("label"));
                sizesAndPrices = new KeyValuePair<string, string>(label.Text,priceString);
            }
            catch (Exception e)
            {
                try
                {
                    IWebElement colors = driver.FindElement(By.Id("pa_color"));
                    IWebElement sizes = driver.FindElement(By.Id("pa_stoerrelse_ml"));
                    List<IWebElement> sizeOption = new List<IWebElement>(sizes.FindElements(By.TagName("option")));
                    List<String> sizeStrings = new List<string>();

                    foreach (IWebElement size in sizeOption)
                    {
                        sizeStrings.Add(size.Text);
                    }

                    SelectElement colorsSelect = new SelectElement(colors);
                    colorsSelect.SelectByIndex(1);
                    SelectElement sizesSelect = new SelectElement(sizes);
                    sizesSelect.SelectByIndex(1);
                
                    sizesAndPrices = new KeyValuePair<string, string>(sizeStrings[1],priceString);
                }
                catch (Exception exception)
                {
                    sizesAndPrices = new KeyValuePair<string, string>("No data",priceString);
                }
            }

            driver.Quit();
            product.Size = sizesAndPrices.Key;
            product.Price = sizesAndPrices.Value;
            return product;
        }
        
        private List<string> GetLinks(IWebDriver driver)
        {
            HashSet<String> hrefs = new HashSet<string>();
            List<IWebElement> a = driver.FindElements(By.ClassName("woocommerce-LoopProduct-link")).ToList();
            foreach (IWebElement webElement in a)
            {
                hrefs.Add(webElement.GetAttribute("href"));
            }

            return hrefs.ToList();
        }

        private List<String> GetNames(IWebDriver driver)
        {
            List<String> names = new List<string>();
            List<IWebElement> namesElements =
                driver.FindElements(By.ClassName("woocommerce-loop-product__title")).ToList();
            foreach (IWebElement name in namesElements)
            {
                names.Add(name.Text);
                Console.WriteLine(name.Text);
            }

            return names;
        }

        private void CleanWindows(IWebDriver driver)
        {
            Thread.Sleep(4000);
            try
            {
                IWebElement cookieElement = driver.FindElement(By.Id("cookie-notice"));
                IJavaScriptExecutor js = (IJavaScriptExecutor) driver;
                js.ExecuteScript("arguments[0].remove()", cookieElement);
            }
            catch (Exception e)
            {
                Console.WriteLine("No need to clear the window");
            }
        }

        private List<String> GetPages(String urlToScrap)
        {
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(urlToScrap);
            CleanWindows(driver);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            Thread.Sleep(4000);
            List<String> pages = new List<string>();
            List<IWebElement> pagination = driver.FindElements(By.ClassName("page-numbers")).ToList();
            if (pagination.Count > 1)
            {
                pagination.RemoveAt(0);
                pagination.RemoveAt(pagination.Count - 1);


                int i = 0;
                pages.Add(urlToScrap);
                foreach (IWebElement page in pagination)
                {
                    if (i != 0)
                    {
                        Console.WriteLine(page.Text);
                        Console.WriteLine(page.GetAttribute("href"));
                        pages.Add(page.GetAttribute("href"));
                    }

                    i++;
                }
            }

            driver.Quit();
            return pages;
        }
    }
}