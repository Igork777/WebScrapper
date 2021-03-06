using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.Enums;
using WebScrapper.Scraping.ScrappingFluggerDk.Repositories;

namespace WebScrapper.Scraping.FluggerHorsensDk
{
    public class FluggerHorsensDkScrapper
    {
        private List<Product> _productsIndoor;
        private List<Product> _productsOutdoor;
        private List<Product> _productsTool;
        private List<Product> _productsOther;
        private UnitOfWork _unitOfWork;

        public FluggerHorsensDkScrapper(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _productsIndoor = new List<Product>();
            _productsOutdoor = new List<Product>();
            _productsTool = new List<Product>();
            _productsOther = new List<Product>();
        }

        public void StartScrapping()
        {
            Console.WriteLine("Starting new scrap");

            _unitOfWork.ProductsType.AddRange(ScrappingHelper._allProductTypes);
            _unitOfWork.Complete();
            _unitOfWork.Website.Add(ScrappingHelper._allWebsites[3]);
            _unitOfWork.Complete();
            _productsIndoor.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/vaegmaling/", TypesOfProduct.Indoors));
            // _productsIndoor.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/traemaling-indendoers/", TypesOfProduct.Indoors));
            //
            // _productsIndoor.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/sproejtemaling/", TypesOfProduct.Indoors));
            // _productsIndoor.AddRange( Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/loftmaling/", TypesOfProduct.Indoors));
            // _productsIndoor.AddRange( Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/indendoers-grunder/", TypesOfProduct.Indoors));
            // _productsIndoor.AddRange( Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/sandmaling/", TypesOfProduct.Indoors));
            // _productsIndoor.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/indfarvet-spartelmasse-indendoers-maling/", TypesOfProduct.Indoors));
            // _productsIndoor.AddRange( Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/metalmaling/", TypesOfProduct.Indoors));
            // _productsIndoor.AddRange( Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/vaegbeklaedning/", TypesOfProduct.Indoors));
            // _productsIndoor.AddRange( Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/gulvmaling/", TypesOfProduct.Indoors));
            // _productsIndoor.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/radiatormaling/", TypesOfProduct.Indoors));
            //
            // _productsOutdoor.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/", TypesOfProduct.Outdoors));
            // _productsOutdoor.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/udendoers-grunder/", TypesOfProduct.Outdoors));
            // _productsOutdoor.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/traemaling-udendoers/", TypesOfProduct.Outdoors));
            // _productsOutdoor.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/vinduesmaling/", TypesOfProduct.Outdoors));
            //
            //
            // _productsOther.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/kit/", TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/kit/linoliekit/", TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/silikonefugemasse/", TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/acrylfugemasse/", TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/", TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/vaadrumsfugemasse/", TypesOfProduct.Others));
            //
            // _productsOther.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningsfilt/", TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningstape/", TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningspap/", TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningsplast/", TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/udendoers-afdaekningspap/", TypesOfProduct.Others));
            //
            // _productsTool.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/malerruller/", TypesOfProduct.Tools));
            // _productsTool.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/rulleskafter/", TypesOfProduct.Tools));
            // _productsTool.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/malerspande/", TypesOfProduct.Tools));
            // _productsTool.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/malerbakker/", TypesOfProduct.Tools));
            // _productsTool.AddRange( Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/slibevaertoej/", TypesOfProduct.Tools));
            // _productsTool.AddRange( Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/rengoering/arbejdshandsker/", TypesOfProduct.Tools));
            // _productsTool.AddRange(Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/spartler/", TypesOfProduct.Tools));
            // _productsTool.AddRange( Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/fugepistoler/", TypesOfProduct.Tools));
            
            
            PopulateProducts(_productsIndoor, ScrappingHelper._allProductTypes[0]);
            _unitOfWork.Complete();
            
            // PopulateProducts(_productsOutdoor, ScrappingHelper._allProductTypes[1]);
            // PopulateProducts(_productsTool, ScrappingHelper._allProductTypes[2]);
            // PopulateProducts(_productsOther, ScrappingHelper._allProductTypes[3]);

        }

        private IList<Product> Start(String urlToScrap, Enum type)
        {
            HtmlDocument htmlDocument = ScrappingHelper.GetHtmlDocument(urlToScrap);
            return GetAllItems(urlToScrap);
        }
        
        private void PopulateProducts(IList<Product> products, ProductType productType)
        {
            foreach (var product in products)
            {
                product.Website = ScrappingHelper._allWebsites[3];
                product.ProductType = productType;
                _unitOfWork.Products.Add(product);
            }
        }

        private List<Product> GetScrappedProducts(String name, String link)
        {
            ChromeOptions options = new ChromeOptions();
            using (IWebDriver driver = new ChromeDriver(options))
            {
                String progress;
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                driver.Navigate()
                    .GoToUrl(link);
                IWebElement defaultColor = driver.FindElement(By.ClassName("color-box"));
                defaultColor.Click();
                Console.WriteLine();
            }

            return null;
        }


        private List<Product> GetAllItems(String urlToScrap)
        {
            ChromeOptions options = new ChromeOptions();
            ReadOnlyCollection<IWebElement> items;
            List<Product> products = new List<Product>();
            using (IWebDriver driver = new ChromeDriver(options))
            {
                driver.Navigate()
                    .GoToUrl(urlToScrap);
                CleanWindow(driver);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
                IWebElement progressOfItems = driver.FindElement(By.Id("progress_count"));
                IWebElement showMoreButton = driver.FindElement(By.Id("loadmore"));
                while (!(AreAllItemsDisplayed(progressOfItems.Text)))
                {
                    Thread.Sleep(4000);
                    showMoreButton.Click();
                    progressOfItems = driver.FindElement(By.Id("progress_count"));
                }


                items = driver.FindElements(By.ClassName("woocommerce-LoopProduct-link"));
                Regex priceRegex = new Regex("[1-9]([0-9]{0,2}|\\.)+,");
                for (int i = 0; i < items.Count; i++)
                {
                    String name = GetName(items[i]);
                    String link = GetLink(items[i]);
                    Console.WriteLine("Name: " + name);
                    Dictionary<String, String> sizeAndPrice = GetSizeAndPrice(link);
                    foreach (KeyValuePair<String, String> iterator in sizeAndPrice)
                    {
                        Product product = new Product();
                        product.Name = name;
                        string size = iterator.Key.Replace(",",".");
                        string price = priceRegex.Match(iterator.Value).Value.Replace(",", "");
                        product.Size = size;
                        product.Price = price;
                        products.Add(product);
                    }
                }

                driver.Quit();
            }

            return products; //return items;
        }


        private Dictionary<String, String> GetSizeAndPrice(String link)
        {
            Dictionary<String, String> SizeAndPrice = new Dictionary<String, string>();
            ChromeOptions options = new ChromeOptions();

            using (IWebDriver driver = new ChromeDriver(options))
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor) driver;
                driver.Navigate()
                    .GoToUrl(link);
                CleanWindow(driver);
                ReadOnlyCollection<IWebElement> colors = driver.FindElements(By.ClassName("color-box"));
                ReadOnlyCollection<IWebElement> allSizes = driver.FindElements(By.ClassName("amount-box"));
                Thread.Sleep(4000);
                if (colors.Count > 1)
                {
                    colors[1].Click();
                    try
                    {
                        colors[0].Click();
                    }
                    catch (Exception ex)
                    {
                        var popup = driver.FindElement(By.ClassName("flugger-close-popup"));
                        popup.Click();
                        Thread.Sleep(4000);
                        colors[0].Click();
                    }
                }

                if (colors.Count == 1)
                {
                    Console.WriteLine("Stop here");
                }

                Regex activeElement = new Regex("active");
                List<IWebElement> selectedSize = GetAllSizes(allSizes);
                Thread.Sleep(4000);
                foreach (IWebElement size in selectedSize)
                {
                    if (!activeElement.IsMatch(size.GetAttribute("class")))
                    {
                        size.Click();
                    }
                    Thread.Sleep(2000);
                    IWebElement price;
                    try
                    {
                        price =
                            driver.FindElements(By.ClassName("woocommerce-Price-amount"))[1];
                    }
                    catch
                    {
                        price =
                            driver.FindElements(By.ClassName("woocommerce-Price-amount"))[0];
                    }
                    Console.WriteLine("Size: " + size.Text+ " Price " + price.Text);
                    SizeAndPrice.Add(size.Text, price.Text);
                }

                driver.Quit();
            }


            return SizeAndPrice;
        }


        private List<IWebElement> GetAllSizes(ReadOnlyCollection<IWebElement> allSizes)
        {
            Regex inactiveItemRegex = new Regex("inactive");
            List<IWebElement> selectedSizes = new List<IWebElement>();
            foreach (IWebElement size in allSizes)
            {
                if (inactiveItemRegex.IsMatch(size.GetAttribute("class")) ||
                    size.GetAttribute("id").Equals("amount-picker"))
                {
                    continue;
                }

                selectedSizes.Add(size);
            }

            return selectedSizes;
        }

        private void CleanWindow(IWebDriver driver)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            IWebElement firstWindow =
                driver.FindElement(By.Id("CybotCookiebotDialogBody"));

            IJavaScriptExecutor js = (IJavaScriptExecutor) driver;
            js.ExecuteScript("arguments[0].remove()", firstWindow);
            Thread.Sleep(5000);
            ReadOnlyCollection<IWebElement> forms = driver.FindElements(By.TagName("form"));
            js.ExecuteScript("arguments[0].remove()", forms[^1]);
        }


        private bool AreAllItemsDisplayed(String progress)
        {
            Console.WriteLine("Progress: " + progress);
            Regex numberRegex = new Regex("[0-9]+");
            MatchCollection matchCollection = numberRegex.Matches(progress);
            if (matchCollection.Count != 2)
            {
                return false;
            }

            int seenItems = Int32.Parse(matchCollection[0].Value);
            int allItems = Int32.Parse(matchCollection[1].Value);

            return seenItems >= allItems;
        }


        private String GetName(IWebElement product)
        {
            return product.FindElement(By.ClassName("woocommerce-loop-product__title")).Text;
        }


        private String GetLink(IWebElement product)
        {
            return product.GetAttribute("href");
        }
    }
}