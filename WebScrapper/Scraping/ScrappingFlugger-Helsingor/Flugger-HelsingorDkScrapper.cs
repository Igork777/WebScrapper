using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Scraping.ScrappingFluggerDk.Enums;


namespace WebScrapper.Scraping
{
    public class FluggerHelsingorDkScrapper
    {
        private DBContext _dbContext;
        private IWebDriver _driver, _driverItem;


        public FluggerHelsingorDkScrapper(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.String")]
        public void StartScrapping()
        {
            // Start(
            //     "https://flugger-helsingor.dk/vare-kategori/indendoers-maling/grundere-indendors-maling/",
            //     TypesOfProduct.Indoors);
            //
            // Start("https://flugger-helsingor.dk/vare-kategori/udendors-maling/facade/",
            //     TypesOfProduct.Outdoors);
            // Start(
            //     "https://flugger-helsingor.dk/vare-kategori/udendors-maling/traebeskyttelse-og-metalmaling/",
            //     TypesOfProduct.Outdoors);
            // Start(
            //     "https://flugger-helsingor.dk/vare-kategori/udendors-maling/grundere-udendors-maling/",
            //     TypesOfProduct.Outdoors);
            //
            //
            // Start("https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvmaling/",
            //     TypesOfProduct.Others);
            // Start(
            //     "https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvolie-gulvbehandling/",
            //     TypesOfProduct.Others);
            // Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/moebelpleje/",
            //     TypesOfProduct.Others);
            // Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/linolier/",
            //     TypesOfProduct.Others);
            // Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/bejdse/",
            //     TypesOfProduct.Others);
            //
            // Start("https://flugger-helsingor.dk/vare-kategori/tilbehor/rengoring-og-afdaekning/",
            //     TypesOfProduct.Tools);
            // Start("https://flugger-helsingor.dk/vare-kategori/tilbehor/spartel-og-fuge/",
            //     TypesOfProduct.Tools);
            // Start(
            //     "https://flugger-helsingor.dk/vare-kategori/tilbehor/vaegbeklaedning-og-klaebere/",
            //     TypesOfProduct.Tools);
            // Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/bakker-og-spande/",
            //     TypesOfProduct.Tools);
            // Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/malerruller/",
            //     TypesOfProduct.Tools);
            // Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/pensler/",
            //     TypesOfProduct.Tools);
            // Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/spartler-og-skrabere/",
            //     TypesOfProduct.Others);
            // Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/vaegbeklaedning/",
            //     TypesOfProduct.Tools);
            // Start(
            //     "https://flugger-helsingor.dk/vare-kategori/indendoers-maling/trae-og-metal/",
            //     TypesOfProduct.Indoors);
            // Start(
            //     "https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvlak-gulvbehandling/",
            //     TypesOfProduct.Others);
            Start("https://flugger-helsingor.dk/vare-kategori/indendoers-maling/loft-og-vaeg/",
                TypesOfProduct.Indoors);
        }

        private void Start(String urlToScrap, Enum type)
        {
            int counter = 0;
            tryAnotherIP:
            try
            {
                _driver = new ChromeDriver();
                _driver.Navigate().GoToUrl(urlToScrap);
                Thread.Sleep(4000);
                CleanWindows(_driver);
                try
                {
                    checkIfConnectionIsOk(_driver);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : " + e);
                    _driver.Quit();
                    goto tryAnotherIP;
                }

                List<String> pages = GetPages(urlToScrap);
                foreach (String page in pages)
                {
                    if (counter == 0)
                    {
                        SaveProduct(urlToScrap,urlToScrap, type);
                    }
                    else
                    {
                        SaveProduct(urlToScrap,page, type);
                    }

                    counter++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e);
                _driver.Quit();
                goto tryAnotherIP;
            }

            _driver.Quit();
        }

        private void SaveProduct(string urlToScrap, string page, Enum type)
        {
            tryAnotherIP:
            try
            {
                try
                {
                    if (!urlToScrap.Equals(page))
                    {
                        _driver.Navigate().GoToUrl(page);
                        CleanWindows(_driver);
                    }
                    checkIfConnectionIsOk(_driver);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : " + e);
                    _driver.Quit();
                    _driver = new ChromeDriver();
                    _driver.Navigate().GoToUrl(page);
                    CleanWindows(_driver);
                    goto tryAnotherIP;
                }

              

                List<String> link = GetLinks(page);
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
                Console.WriteLine("Exception : " + ex);
                _driverItem.Quit();
                _driverItem = new ChromeDriver();
                _driverItem.Navigate().GoToUrl(page);
                goto tryAnotherIP;
            }
        }

        private String GetImagePath(IWebDriver driver)
        {
            Thread.Sleep(4000);
            IWebElement pathToImage = driver.FindElement(By.ClassName("woocommerce-product-gallery__wrapper"));
            String href = "No data";
            try
            {
                Thread.Sleep(4000);
                IWebElement a = pathToImage.FindElement(By.TagName("a"));
                href = a.GetAttribute("href");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e);
                Console.WriteLine("No link for image exists");
            }

            ;
            return href;
        }

        private IList<Product> GetProduct(String link)
        {
            tryAnotherIP:
            Regex priceRegex = new Regex("[1-9]([0-9]{0,2}|\\.)+,");
            IList<Product> products = new List<Product>();
            try
            {
                _driverItem = new ChromeDriver();
                _driverItem.Navigate().GoToUrl(link);
                CleanWindows(_driverItem);

                Thread.Sleep(4000);
                IWebElement nameElement = _driverItem.FindElement(By.ClassName("et_pb_module_inner"));
                String name = ScrappingHelper.RemoveDiacritics(nameElement.Text.Trim());
                if (name.Contains("Tonet"))
                {
                    _driverItem.Quit();
                    return new List<Product>();
                }

                Thread.Sleep(4000);
                IWebElement price = _driverItem.FindElement(By.ClassName("price"));
                IWebElement pr = null;
                String priceString = "";
                try
                {
                    Thread.Sleep(4000);
                    pr = price.FindElement(By.TagName("ins"));
                    priceString = priceRegex.Match(pr.Text).Value.Replace(",", "");
                    Console.WriteLine("Price: " + priceString);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : " + e);
                    Console.WriteLine("No old price");
                    priceString = priceRegex.Match(price.Text).Value.Replace(",", "");
                }

                try
                {
                    Thread.Sleep(4000);
                    IWebElement colorsRadio = _driverItem.FindElement(By.Id("picker_pa_color"));
                    Thread.Sleep(4000);
                    List<IWebElement> optionss = colorsRadio.FindElements(By.ClassName("select-option")).ToList();
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
                    Thread.Sleep(4000);
                    IWebElement ulOfRadios = _driverItem.FindElement(By.Id("picker_pa_stoerrelse_ml"));
                    Thread.Sleep(4000);
                    List<IWebElement> Lis = ulOfRadios.FindElements(By.TagName("li")).ToList();
                    if (Lis.Count != 1)
                    {
                        for (int i = 0; i < Lis.Count; i++)
                        {
                            Product product = new Product();
                            product.PathToImage = GetImagePath(_driverItem);
                            product.Name = name;
                            tryToClick:
                            try
                            {
                                Thread.Sleep(4000);
                                IWebElement radioOption = Lis[i].FindElement(By.ClassName("radio-option"));
                                radioOption.Click();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                Thread.Sleep(4000);
                                goto tryToClick;
                            }

                            tryAnotherIns:

                            Thread.Sleep(4000);
                            IWebElement arr = _driverItem.FindElement(By.ClassName("single_variation_wrap"));
                            Thread.Sleep(4000);
                            IWebElement web = arr.FindElement(By.ClassName("price"));
                            Thread.Sleep(4000);
                            try
                            {
                                priceString = web.FindElement(By.TagName("ins")).Text;
                            }
                            catch (Exception e)
                            {
                                priceString = web.Text;
                            }

                            priceString = priceRegex.Match(priceString).Value.Replace(",", "");

                            product.Size = Lis[i].Text;
                            product.Price = priceString;
                            products.Add(product);
                        }
                    }
                    else
                    {
                        Product product = new Product();
                        product.PathToImage = GetImagePath(_driverItem);
                        product.Name = name;
                        product.Size = Lis[0].Text;
                        product.Price = priceString;
                        products.Add(product);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : " + e);
                    try
                    {
                        IWebElement colors = _driverItem.FindElement(By.Id("pa_color"));
                        IWebElement sizes = _driverItem.FindElement(By.Id("pa_stoerrelse_ml"));

                        List<IWebElement> sizeOption = new List<IWebElement>(sizes.FindElements(By.TagName("option")));
                        List<String> sizeStrings = new List<string>();

                        foreach (IWebElement size in sizeOption)
                        {
                            Console.WriteLine(size.Text);
                            sizeStrings.Add(size.Text);
                        }

                        SelectElement colorsSelect = new SelectElement(colors);
                        SelectElement sizesSelect = new SelectElement(sizes);
                        colorsSelect.SelectByIndex(1);

                        for (int i = 1; i < sizeStrings.Count; i++)
                        {
                            sizesSelect.SelectByIndex(i);
                            Product product = new Product();
                            product.PathToImage = GetImagePath(_driverItem);
                            product.Name = name;


                            Thread.Sleep(4000);
                            IWebElement arr = _driverItem.FindElement(By.ClassName("single_variation_wrap"));
                            Thread.Sleep(4000);
                            IWebElement web = arr.FindElement(By.ClassName("price"));
                            Thread.Sleep(4000);
                            string priceAsString = web.FindElement(By.TagName("ins")).Text;
                            priceAsString = priceRegex.Match(priceAsString).Value.Replace(",", "");
                            product.Size = sizeStrings[i];
                            product.Price = priceAsString;
                            products.Add(product);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception : " + ex);
                        Product product = new Product();
                        product.PathToImage = GetImagePath(_driverItem);
                        product.Name = name;
                        product.Size = "No data";
                        product.Price = priceString;
                        products.Add(product);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e);
                _driverItem.Quit();
                _driverItem = new ChromeDriver();
                _driverItem.Navigate().GoToUrl(link);
                goto tryAnotherIP;
            }

            _driverItem.Quit();
            return products;
        }

        private List<string> GetLinks(string urlToScrap)
        {
            tryAnotherIP:

            HashSet<String> hrefs = new HashSet<string>();
            try
            {
                Thread.Sleep(4000);
                List<IWebElement> a = _driver.FindElements(By.ClassName("woocommerce-LoopProduct-link")).ToList();
                foreach (IWebElement webElement in a)
                {
                    hrefs.Add(webElement.GetAttribute("href"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e);
                _driver.Quit();
                _driver = new ChromeDriver();
                _driver.Navigate().GoToUrl(urlToScrap);
                goto tryAnotherIP;
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
                Console.WriteLine("Exception : " + e);
                Console.WriteLine("No need to clear the window");
            }
        }

        private List<String> GetPages(String urlToScrap)
        {
            HashSet<String> pages = new HashSet<string>();
            tryAnotherIP:
            try
            {
                try
                {
                    checkIfConnectionIsOk(_driver);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : " + e);
                    _driver.Quit();
                    _driver = new ChromeDriver();
                    _driver.Navigate().GoToUrl(urlToScrap);
                    CleanWindows(_driver);
                    goto tryAnotherIP;
                }

                Thread.Sleep(4000);
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e);
                _driver.Quit();
                _driver = new ChromeDriver();
                _driver.Navigate().GoToUrl(urlToScrap);
                goto tryAnotherIP;
            }

            return pages.ToList();
        }

        private void tryAnotherProxy()
        {
            Thread.Sleep(4000);

            _driver.Quit();
            _driverItem.Quit();

            _driver = new ChromeDriver();
            _driverItem = new ChromeDriver();
        }


        private void checkIfConnectionIsOk(IWebDriver driver)
        {
            Thread.Sleep(4000);
            driver.FindElement(By.Id("main-header"));
        }
    }
}