using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using WebJobStarter.DbContext;
using WebJobStarter.DTO;
using WebJobStarter.Enums;
using WebJobStarter.Helpers;

namespace WebJobStarter
{
    public class Flugger_HelsingorDkScrapper
    {
        private DBContext _dbContext;
        private IWebDriver _driver;
        private ChromeOptions options;


        public Flugger_HelsingorDkScrapper(DBContext dbContext)
        {
            _dbContext = dbContext;
            options = ScrappingHelper.getOptions();
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.String")]
        public void StartScrapping()
        {
            Console.WriteLine("Starting new scrap : FluggerHelsingorDkScrapper");
            _driver = new RemoteWebDriver(new Uri("https://chrome.browserless.io/webdriver"), options.ToCapabilities());
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Start(
                "https://flugger-helsingor.dk/vare-kategori/indendoers-maling/",
                TypesOfProduct.Indoors);

            Start("https://flugger-helsingor.dk/vare-kategori/udendors-maling/",
                TypesOfProduct.Outdoors);

            Start(
                "https://flugger-helsingor.dk/vare-kategori/gulvbehandling/",
                TypesOfProduct.Others);
            Start(
                "https://flugger-helsingor.dk/vare-kategori/tilbehor/",
                TypesOfProduct.Others);
            _driver.Quit();
        }

        private void Start(String urlToScrap, Enum type)
        {
            _driver.Navigate().GoToUrl(urlToScrap);
            CleanWindows(_driver);
            int counter = 1;
            List<String> pages = GetPages(urlToScrap);
            foreach (String page in pages)
            {
              
                    saveProductAgain:
                    try
                    {
                        if (page == null)
                        {
                            SaveProduct(urlToScrap, urlToScrap, type);
                        }
                        else
                        {
                            SaveProduct(urlToScrap, page, type);
                        }
                    }
                    catch (WebDriverException webDriverException)
                    {
                        _driver?.Quit();
                        _driver = null;
                        _driver = new RemoteWebDriver(new Uri("https://chrome.browserless.io/webdriver"), options.ToCapabilities());
                        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                        _driver.Navigate().GoToUrl(urlToScrap);
                        goto saveProductAgain;
                    }

                    _driver.Quit();

                    counter++;
            }
        }

        private void SaveProduct(string urlToScrap, string page, Enum type)
        {
            CleanWindows(_driver);
            if (!urlToScrap.Equals(page))
            {
                goBack2:
                try
                {
                    _driver.Navigate().GoToUrl(page);
                }
                catch (WebDriverException ex)
                {
                    _driver.Navigate().Back();
                    CleanWindows(_driver);
                    goto goBack2;
                }

                CleanWindows(_driver);
            }


            List<String> linksToProducts = GetLinks(page);
            for (int i = 0; i < linksToProducts.Count; i++)
            {
                IList<Product> products = GetProduct(linksToProducts[i]);
                foreach (Product product in products)
                {
                    product.ProductTypeId = Convert.ToInt32(type);
                    product.WebsiteId = 2;
                    Console.WriteLine("Product name: " + product.Name);
                    Console.WriteLine("Product size:" + product.Size);
                    Console.WriteLine("Product price: " + product.CurrentPrice);
                    ScrappingHelper.SaveOrUpdate(_dbContext, product);
                }
            }
        }

        private String GetImagePath()
        {
            IWebElement pathToImage = _driver.FindElement(By.ClassName("woocommerce-product-gallery__wrapper"));
            String href = "No data";
            try
            {
              //  Thread.Sleep(2000);
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
            IList<Product> products = new List<Product>();

            goBack:
            try
            {
                // var chromeOptions = new ChromeOptions();
                // chromeOptions.AddArguments("headless");
                // _driver = new ChromeDriver(chromeOptions);
                _driver.Navigate().GoToUrl(link);
                CleanWindows(_driver);
            }
            catch (WebDriverException e)
            {
                Console.WriteLine(e.StackTrace);
                _driver.Navigate().Back();
                CleanWindows(_driver);
                goto goBack;
            }


            IWebElement nameElement = _driver.FindElement(By.ClassName("et_pb_module_inner"));
            String name = ScrappingHelper.RemoveDiacritics(nameElement.Text.Trim()).Replace("Flugger ", "");
            if (name.Contains("Tonet"))
            {
                _driver.Navigate().Back();
                return new List<Product>();
            }

            IWebElement price = _driver.FindElement(By.ClassName("price"));
            IWebElement pr = null;
            String priceString = "";
            try
            {
                //Thread.Sleep(2000);
                pr = price.FindElement(By.TagName("ins"));
                priceString = priceRegex.Match(pr.Text).Value.Replace(",", ".").Replace(" ", "").Replace("L", "")
                    .Replace(".", "");
                Console.WriteLine("CurrentPrice: " + priceString);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e);
                Console.WriteLine("No old price");
                priceString = priceRegex.Match(price.Text).Value.Replace(",", ".").Replace(" ", "").Replace("L", "")
                    .Replace(".", "");
            }

            List<IWebElement> optionss = new List<IWebElement>();
            try
            {
                IWebElement colorsRadio = _driver.FindElement(By.Id("picker_pa_color"));
                //Thread.Sleep(2000);
                optionss = colorsRadio.FindElements(By.ClassName("select-option")).ToList();
                if (optionss.Count > 1)
                {
                    optionss[1].Click();
                    optionss[0].Click();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex);
            }

            try
            {
                IWebElement ulOfRadios = _driver.FindElement(By.Id("picker_pa_stoerrelse_ml"));
                //Thread.Sleep(2000);
                List<IWebElement> Lis = ulOfRadios.FindElements(By.TagName("li")).ToList();
                if (Lis.Count != 1)
                {
                    String pathToImage = GetImagePath();
                    for (int i = 0; i < Lis.Count; i++)
                    {
                        Product product = new Product();
                        product.PathToImage = pathToImage;
                        product.Name = name;
                        tryToClick:
                        try
                        {
                            //Thread.Sleep(2000);
                            IWebElement radioOption = Lis[i].FindElement(By.ClassName("radio-option"));
                            radioOption.Click();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                           // Thread.Sleep(2000);
                            goto tryToClick;
                        }


                        IWebElement arr = _driver.FindElement(By.ClassName("single_variation_wrap"));
                       // Thread.Sleep(2000);
                        IWebElement woocommerce_variation_price =
                            arr.FindElement(By.ClassName("woocommerce-variation-price"));
                        IWebElement web = woocommerce_variation_price.FindElement(By.ClassName("price"));
                        try
                        {
                            priceString = web.FindElement(By.TagName("ins")).Text;
                            if (priceString.Equals(""))
                            {
                                optionss[0].Click();
                                //Thread.Sleep(3000);
                                 arr = _driver.FindElement(By.ClassName("single_variation_wrap"));
                                 woocommerce_variation_price =
                                    arr.FindElement(By.ClassName("woocommerce-variation-price"));
                                web = woocommerce_variation_price.FindElement(By.ClassName("price"));
                                priceString = web.FindElement(By.TagName("ins")).Text;
                            }
                        }
                        catch (Exception e)
                        {
                            priceString = web.Text;
                        }

                        priceString = priceRegex.Match(priceString).Value.Replace(",", ".").Replace(" ", "")
                            .Replace("L", "").Replace(".", "");

                        product.Size = Lis[i].Text.Replace(",", ".").Replace(" ", "").Replace("L", "").Replace("ml", "")
                            .Replace("kg", "").Replace("Gram", "");
                        product.CurrentPrice = priceString.Replace(",", ".").Replace(" ", "").Replace("L", "")
                            .Replace(".", "");
                        products.Add(product);
                    }
                }
                else
                {
                    Product product = new Product();
                    product.PathToImage = GetImagePath();
                    product.Name = name;
                    product.Size = Lis[0].Text.Replace(",", ".").Replace(" ", "").Replace("L", "").Replace("ml", "")
                        .Replace("kg", "").Replace("Gram", "");
                    product.CurrentPrice =
                        priceString.Replace(",", ".").Replace(" ", "").Replace("L", "").Replace(".", "");
                    products.Add(product);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e);
                try
                {
                    IWebElement colors = _driver.FindElement(By.Id("pa_color"));
                    IWebElement sizes = _driver.FindElement(By.Id("pa_stoerrelse_ml"));

                    List<IWebElement> sizeOption = new List<IWebElement>(sizes.FindElements(By.TagName("option")));
                    List<String> sizeStrings = new List<string>();

                    foreach (IWebElement size in sizeOption)
                    {
                        Console.WriteLine(size.Text);
                        sizeStrings.Add(size.Text.Replace(",", ".").Replace(" ", "").Replace("L", "").Replace("ml", "")
                            .Replace("kg", "").Replace("Gram", ""));
                    }

                    SelectElement colorsSelect = new SelectElement(colors);
                    SelectElement sizesSelect = new SelectElement(sizes);
                    colorsSelect.SelectByIndex(1);

                    for (int i = 1; i < sizeStrings.Count; i++)
                    {
                        sizesSelect.SelectByIndex(i);
                        Product product = new Product();
                        product.PathToImage = GetImagePath();
                        product.Name = name;


                        IWebElement arr = _driver.FindElement(By.ClassName("single_variation_wrap"));
                       // Thread.Sleep(2000);
                        IWebElement web = arr.FindElement(By.ClassName("price"));
                       // Thread.Sleep(2000);
                        string priceAsString = web.FindElement(By.TagName("ins")).Text;
                        priceAsString = priceRegex.Match(priceAsString).Value.Replace(",", "");
                        product.Size = sizeStrings[i].Replace(",", ".").Replace(" ", "").Replace("L", "");
                        ;
                        product.CurrentPrice = priceAsString.Replace(",", ".").Replace(" ", "").Replace("L", "")
                            .Replace(".", "");
                        products.Add(product);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception : " + ex);
                    Product product = new Product();
                    product.PathToImage = GetImagePath();
                    product.Name = name;
                    product.Size = "No data";
                    product.CurrentPrice =
                        priceString.Replace(",", ".").Replace(" ", "").Replace("L", "").Replace(".", "");
                    products.Add(product);
                }
            }

            _driver.Navigate().Back();
            return products;
        }

        private List<string> GetLinks(string urlToScrap)
        {
            HashSet<String> hrefs = new HashSet<string>();

            List<IWebElement> a = _driver.FindElements(By.ClassName("woocommerce-LoopProduct-link")).ToList();
            foreach (IWebElement webElement in a)
            {
                hrefs.Add(webElement.GetAttribute("href"));
            }

            return hrefs.ToList();
        }

        private void CleanWindows(IWebDriver driver)
        {
            //Thread.Sleep(4000);
            try
            {
                IWebElement cookieElement = driver.FindElement(By.Id("cookie-notice"));
                IJavaScriptExecutor js = (IJavaScriptExecutor) driver;
                js.ExecuteScript("arguments[0].remove()", cookieElement);
                js.ExecuteScript("document.querySelector(`[data-testid='dialog_iframe']`).style.zIndex = 0");
            }
            catch (Exception e)
            {
                Console.WriteLine("No need to clear the window");
            }
        }

        private List<String> GetPages(String urlToScrap)
        {
            HashSet<String> pages = new HashSet<string>();
            //Thread.Sleep(4000);
            List<IWebElement> pagination = _driver.FindElements(By.ClassName("page-numbers")).ToList();

            if (pagination.Count <= 2)
            {
                pages.Add(urlToScrap);
            }
            else
            {
                pagination.RemoveAt(0);
                pagination.RemoveAt(pagination.Count - 1);

                foreach (IWebElement page in pagination)
                {
                    Console.WriteLine(page.Text);
                    Console.WriteLine(page.GetAttribute("href"));
                    pages.Add(page.GetAttribute("href"));
                }
            }

            return pages.ToList();
        }
    }
}