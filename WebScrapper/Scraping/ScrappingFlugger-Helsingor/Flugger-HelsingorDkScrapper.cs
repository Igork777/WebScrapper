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
            Start(
                "https://flugger-helsingor.dk/vare-kategori/indendoers-maling/trae-og-metal/",
                TypesOfProduct.Indoors);
            Start(
                "https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvlak-gulvbehandling/",
                TypesOfProduct.Others);
         
            Start("https://flugger-helsingor.dk/vare-kategori/indendoers-maling/loft-og-vaeg/",
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
                SaveProduct(page, type);
            }
        }

        private void SaveProduct(string page, Enum type)
        {
            int tryed_times = 0;
            tryOnceAgain:
            Console.WriteLine("Trying once again");
            IWebDriver driver = null;
            try
            {
                ChromeOptions options = new ChromeOptions();
                driver = new ChromeDriver(options);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
                driver.Navigate().GoToUrl(page);
                CleanWindows(driver);
                List<String> link = GetLinks(driver);
                for (int i = 0; i < link.Count; i++)
                {
                    IList<Product> products = GetProduct(link[i]);
                    foreach (Product product in products)
                    {
                        product.ProductTypeId = Convert.ToInt32(type);
                        product.WebsiteId = 2;
                        Console.WriteLine("Product name: " + product.Name);
                        Console.WriteLine("Product size:" + product.Size);
                        Console.WriteLine("Product price: " + product.Price);
                        ScrappingHelper.SaveOrUpdate(_dbContext, product);
                    }
                }
            }
            catch (Exception ex)
            {
                driver?.Quit();
                if (tryed_times > 10)
                {
                    return;
                }

                tryed_times++;
                goto tryOnceAgain;
            }

            driver?.Quit();
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
            }

            ;
            return href;
        }

        private IList<Product> GetProduct(String link)
        {
            Regex priceRegex = new Regex("[1-9]([0-9]{0,2}|\\.)+,");
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver(options);
            IList<Product> products = new List<Product>();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            driver.Navigate().GoToUrl(link);
            CleanWindows(driver);
            
            IWebElement nameElement = driver.FindElement(By.ClassName("et_pb_module_inner"));
            String name = ScrappingHelper.RemoveDiacritics(nameElement.Text.Trim());
            if (name.Contains("Tonet"))
            {
                driver.Quit();
                return new List<Product>();
            }

            Thread.Sleep(4000);
            IWebElement price = driver.FindElement(By.ClassName("price"));
            IWebElement pr = null;
            String priceString = "";
            try
            {
                pr = price.FindElement(By.TagName("ins"));
                priceString = priceRegex.Match(pr.Text).Value.Replace(",", "");
                Console.WriteLine("Price: " + priceString);
            }
            catch (Exception e)
            {
                Console.WriteLine("No old price");
                priceString = priceRegex.Match(price.Text).Value.Replace(",", "");
                Console.WriteLine("Price: " + priceString);
            }

            // try
            // {
            //     IWebElement radio = driver.FindElement(By.Id("picker_pa_stoerrelse_ml"));
            //     List<IWebElement> labels = radio.FindElements(By.TagName("label")).ToList();
            //     sizesAndPrices.Add(new KeyValuePair<string, string>(label.Text, priceString));
            // }
            // catch (Exception e)
            // {

            try
            {
                IWebElement colorsRadio = driver.FindElement(By.Id("picker_pa_color"));
                List<IWebElement> optionss = colorsRadio.FindElements(By.ClassName("select-option")).ToList();
                if (optionss.Count > 1)
                {
                    optionss[0].Click();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
            }

            try
            {
                IWebElement ulOfRadios = driver.FindElement(By.Id("picker_pa_stoerrelse_ml"));
                List<IWebElement> Lis = ulOfRadios.FindElements(By.TagName("li")).ToList();
                if (Lis.Count != 1)
                {
                    for (int i = 0; i < Lis.Count; i++)
                    {
                        Product product = new Product();
                        product.PathToImage = GetImagePath(driver);
                        product.Name = name;
                       
                        Thread.Sleep(4000);
                        IWebElement radioOption = Lis[i].FindElement(By.ClassName("radio-option"));
                        radioOption.Click();
                        Thread.Sleep(4000);
                        IWebElement arr = driver.FindElement(By.ClassName("single_variation_wrap"));
                        Thread.Sleep(4000);
                        IWebElement web = arr.FindElement(By.ClassName("price"));
                        Thread.Sleep(8000);
                        priceString = web.FindElement(By.TagName("ins")).Text;
                        priceString = priceRegex.Match(priceString).Value.Replace(",", "");
                        
                        product.Size = Lis[i].Text;
                        product.Price = priceString;
                        products.Add(product);
                    }
                }
                else
                { 
                    Product product = new Product();
                    product.PathToImage = GetImagePath(driver);
                    product.Name = name;
                    product.Size = Lis[0].Text;
                    product.Price = priceString;
                    products.Add(product);
                }
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
                        Console.WriteLine(size.Text);
                        sizeStrings.Add(size.Text);
                    }

                    SelectElement colorsSelect = new SelectElement(colors);
                    colorsSelect.SelectByIndex(1);

                    SelectElement sizesSelect = new SelectElement(sizes);
                    for (int i = 1; i < sizeStrings.Count; i++)
                    {
                        Product product = new Product();
                        product.PathToImage = GetImagePath(driver);
                        product.Name = name;
                       
                        
                        Thread.Sleep(4000);
                        IWebElement arr = driver.FindElement(By.ClassName("single_variation_wrap"));

                        IWebElement web = arr.FindElement(By.ClassName("price"));
                        Thread.Sleep(8000);
                        string priceAsString = web.FindElement(By.TagName("ins")).Text;
                        priceAsString = priceRegex.Match(priceAsString).Value.Replace(",", "");
                        product.Size = sizeStrings[i];
                        product.Price = priceAsString;
                        products.Add(product);
                    }
                }
                catch (Exception EX_NAME)
                {
                    Product product = new Product();
                     product.PathToImage = GetImagePath(driver);
                     product.Name = name;
                     product.Size = "No data";
                     product.Price = priceString;
                     products.Add(product);
                }

            }



            //  }

            driver.Quit();
            return products;
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
            IWebDriver driver = null;
            int tryed_times = 0;
            HashSet<String> pages = new HashSet<string>();
            tryOnceAgain:
            try
            {
                ChromeOptions options = new ChromeOptions();
                driver = new ChromeDriver(options);
                driver.Navigate().GoToUrl(urlToScrap);
                CleanWindows(driver);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
                Thread.Sleep(4000);

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
                else
                {
                    pages.Add(urlToScrap);
                }
            }
            catch (Exception e)
            {
                driver?.Quit();
                if (tryed_times > 10)
                {
                    Console.WriteLine("Impossible to retrive data from page: " + urlToScrap);
                    return new List<string>();
                }

                tryed_times++;
                goto tryOnceAgain;
            }


            driver.Quit();
            return pages.ToList();
        }
    }
}