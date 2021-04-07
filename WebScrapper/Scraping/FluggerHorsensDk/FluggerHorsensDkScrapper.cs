using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Scraping.ScrappingFluggerDk.Enums;

namespace WebScrapper.Scraping.FluggerHorsensDk
{
    public class FluggerHorsensDkScrapper
    {
        private DBContext _dbContext;
        int counterOfTries = 0;

        private IWebDriver _driver, _driverItem;

        public FluggerHorsensDkScrapper(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void StartScrapping()
        {
            Console.WriteLine("Starting new scrap");
            ChromeOptions chromeOptions = new ChromeOptions();
            _driver = new ChromeDriver(chromeOptions);
            _driverItem = new ChromeDriver(chromeOptions);
            Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/vaegmaling/",
                TypesOfProduct.Indoors);
            
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/traemaling-indendoers/",
                TypesOfProduct.Indoors);
            
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/sproejtemaling/",
                TypesOfProduct.Indoors);
            
            Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/loftmaling/",
                TypesOfProduct.Indoors);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/malerruller/", TypesOfProduct.Tools);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/rulleskafter/",
                TypesOfProduct.Tools);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/malerspande/", TypesOfProduct.Tools);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/malerbakker/", TypesOfProduct.Tools);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/slibevaertoej/",
                TypesOfProduct.Tools);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/rengoering/arbejdshandsker/",
                TypesOfProduct.Tools);
            Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/spartler/",
                TypesOfProduct.Tools);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/fugepistoler/",
                TypesOfProduct.Tools);
            
            
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/indendoers-grunder/",
                TypesOfProduct.Indoors);
            
            Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/sandmaling/",
                TypesOfProduct.Indoors);
            
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/indfarvet-spartelmasse-indendoers-maling/",
                TypesOfProduct.Indoors);
            
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/metalmaling/", TypesOfProduct.Indoors);
            
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/vaegbeklaedning/",
                TypesOfProduct.Indoors);
            
            Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/gulvmaling/",
                TypesOfProduct.Indoors);
            
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/radiatormaling/",
                TypesOfProduct.Indoors);

            Start("https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/",
                TypesOfProduct.Outdoors);

            Start(
                "https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/udendoers-grunder/",
                TypesOfProduct.Outdoors);

            Start(
                "https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/traemaling-udendoers/",
                TypesOfProduct.Outdoors);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/vinduesmaling/",
                TypesOfProduct.Outdoors);


            Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/kit/",
                TypesOfProduct.Others);
            Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/kit/linoliekit/",
                TypesOfProduct.Others);

            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/silikonefugemasse/",
                TypesOfProduct.Others);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/acrylfugemasse/",
                TypesOfProduct.Others);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/",
                TypesOfProduct.Others);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/vaadrumsfugemasse/",
                TypesOfProduct.Others);

            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningsfilt/",
                TypesOfProduct.Others);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningstape/",
                TypesOfProduct.Others);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningspap/",
                TypesOfProduct.Others);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningsplast/",
                TypesOfProduct.Others);
            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/udendoers-afdaekningspap/",
                TypesOfProduct.Others);
            _driverItem.Quit();
            _driver.Quit();
        }

        private void Start(String urlToScrap, Enum type)
        {
            GetAllItems(urlToScrap, type);
        }


        private void GetAllItems(string urlToScrap, Enum type)
        {
            ReadOnlyCollection<IWebElement> items;


            tryOneMoreTime:
            try
            {
                _driver.Navigate()
                    .GoToUrl(urlToScrap);
                CleanWindow(_driver);
                IWebElement progressOfItems = null;
                Thread.Sleep(4000);
                progressOfItems = _driver.FindElement(By.Id("progress_count"));
                IWebElement showMoreButton = null;
                Thread.Sleep(4000);
                showMoreButton = _driver.FindElement(By.Id("loadmore"));

                String previousCount = "";
                while (!AreAllItemsDisplayed(progressOfItems.Text))
                {
                    showMoreButton.Click();
                    Thread.Sleep(4000);
                    progressOfItems = _driver.FindElement(By.Id("progress_count"));


                    if (progressOfItems.Text.Equals(previousCount))
                    {
                        break;
                    }

                    previousCount = progressOfItems.Text;
                }

                Thread.Sleep(4000);
                items = _driver.FindElements(By.ClassName("woocommerce-LoopProduct-link"));
            }
            catch (Exception ex)
            {
                goto tryOneMoreTime;
            }

            Regex priceRegex = new Regex("[1-9]([0-9]{0,2}|\\.)+,");
            for (int j = 0; j < items.Count; j++)
            {
                String name = ScrappingHelper.RemoveDiacritics(GetName(items[j]));
                String link = GetLink(items[j]);


                Console.WriteLine("Name: " + name);
                Dictionary<String, String> sizeAndPrice = GetSizeAndPrice(link);
                Product product = null;
                String pathToImage = GetPathToTheImage(link);
                foreach (KeyValuePair<String, String> i in sizeAndPrice)
                {
                    product = new Product();
                    product.Name = name;
                    string size = i.Key.Replace(",", ".");
                    string price = priceRegex.Match(i.Value).Value.Replace(",", "");
                    product.Size = size;
                    product.Price = price;
                    product.PathToImage = pathToImage;
                    product.ProductTypeId = Convert.ToInt32(type);
                    product.WebsiteId = 4;

                    ScrappingHelper.SaveOrUpdate(_dbContext, product);
                }

                Console.WriteLine("Product: " + name + " finished");
            }
        }


        private String GetPathToTheImage(string link)
        {
            Regex pngImage = new Regex("http.*?\\.png");
            String finishedString = "";
            tryPneMoreTime:
            try
            {
                Thread.Sleep(4000);
                try
                {
                    _driverItem.FindElement(By.Id("main_header"));


                    String compoundSrcSet = "";
                    Thread.Sleep(4000);
                    IWebElement path = _driverItem.FindElement(By.ClassName("attachment-product-image"));
                    compoundSrcSet = path.GetAttribute("srcset");


                    String[] sets = compoundSrcSet.Split(",");
                    finishedString = "No data";
                    foreach (String set in sets)
                    {
                        if (set.Contains(".png"))
                        {
                            finishedString = pngImage.Match(set).Value;
                            Console.WriteLine(finishedString);
                            break;
                        }
                    }
                }
                catch (NoSuchElementException e)
                {
                    return finishedString;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _driverItem.Quit();
                _driverItem = new ChromeDriver(new ChromeOptions());
                goto tryPneMoreTime;
            }


            _driverItem.Quit();
            return finishedString;
        }


        private Dictionary<String, String> GetSizeAndPrice(String link)
        {
            Dictionary<String, String> SizeAndPrice;
            tryPneMoreTime:
            try
            {
                SizeAndPrice = new Dictionary<String, string>();
                
                _driverItem.Navigate()
                    .GoToUrl(link);
                Thread.Sleep(4000);
                _driverItem.FindElement(By.Id("main_header"));
                ReadOnlyCollection<IWebElement> colors;
                Thread.Sleep(4000);
                colors = _driverItem.FindElements(By.ClassName("color-box"));
                CleanWindow(_driverItem);
                ReadOnlyCollection<IWebElement> allSizes;
                Thread.Sleep(4000);
                allSizes = _driverItem.FindElements(By.ClassName("amount-box"));


                if (colors.Count > 1)
                {
                    try
                    {
                        colors[1].Click();
                        colors[0].Click();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Thread.Sleep(4000);
                        
                        
                        var popup = _driverItem.FindElement(By.ClassName("flugger-close-popup"));
                        popup.Click();
                        colors[0].Click();
                    }
                }

                if (colors.Count == 1)
                {
                    Console.WriteLine("Stop here");
                }

                Regex activeElement = new Regex("active");
                Regex sizeRegex = new Regex("([0-9]{0,2}|,)[0-9]+");
                List<IWebElement> selectedSize = GetAllSizes(allSizes);
                List<String> prices = new List<String>();
                foreach (IWebElement size in selectedSize)
                {
                    if (!activeElement.IsMatch(size.GetAttribute("class")))
                    {
                        size.Click();
                    }


                    IWebElement price;
                    try
                    {
                        Thread.Sleep(4000);
                        price =
                            _driverItem.FindElements(By.ClassName("woocommerce-Price-amount"))[1];
                    }
                    catch
                    {
                        Thread.Sleep(4000);
                        price =
                            _driverItem.FindElements(By.ClassName("woocommerce-Price-amount"))[0];
                    }

                    if (prices.Contains(price.Text))
                    {
                        continue;
                    }

                    if (sizeRegex.IsMatch(size.Text))
                    {
                        SizeAndPrice.Add(size.Text, price.Text);
                        prices.Add(price.Text);
                        Console.WriteLine("Size: " + size.Text + " Price " + price.Text);
                    }
                    else if (size.Text.Equals(""))
                    {
                        SizeAndPrice.Add("No data", price.Text);
                        prices.Add(price.Text);
                        Console.WriteLine("Size: " + size.Text + " Price " + price.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _driverItem.Quit();
                _driverItem = new ChromeDriver(new ChromeOptions());
                goto tryPneMoreTime;
            }

            return SizeAndPrice;
        }


        private List<IWebElement> GetAllSizes(ReadOnlyCollection<IWebElement> allSizes)
        {
            Regex inactiveItemRegex = new Regex("inactive");
            List<IWebElement> selectedSizes = null;
            tryOneMoreTime:

            selectedSizes = new List<IWebElement>();
            foreach (IWebElement size in allSizes)
            {
                tryAmountPicker:
                try
                {
                    if (inactiveItemRegex.IsMatch(size.GetAttribute("class")) ||
                        size.GetAttribute("id").Equals("amount-picker"))
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    if (counterOfTries < 3)
                    {
                        Thread.Sleep(4000);
                        counterOfTries++;
                        goto tryAmountPicker;
                    }

                    counterOfTries = 0;
                    throw;
                }

                selectedSizes.Add(size);
            }


            return selectedSizes;
        }

        private void CleanWindow(IWebDriver driver)
        {
            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
                IWebElement firstWindow =
                    driver.FindElement(By.Id("CybotCookiebotDialogBody"));

                IJavaScriptExecutor js = (IJavaScriptExecutor) driver;
                js.ExecuteScript("arguments[0].remove()", firstWindow);
                Thread.Sleep(5000);
                ReadOnlyCollection<IWebElement> forms = driver.FindElements(By.TagName("form"));
                js.ExecuteScript("arguments[0].remove()", forms[^1]);
                ReadOnlyCollection<IWebElement> iframes = driver.FindElements(By.TagName("iframe"));
                js.ExecuteScript("argument[0].remove()", iframes[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Clear windows not needed");
            }
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
            String name = "";
            Thread.Sleep(4000);
            name = product.FindElement(By.ClassName("woocommerce-loop-product__title")).Text;

            name = name.Replace("Flügger ", "");
            return name;
        }


        private String GetLink(IWebElement product)
        {
            return product.GetAttribute("href");
        }
    }
}