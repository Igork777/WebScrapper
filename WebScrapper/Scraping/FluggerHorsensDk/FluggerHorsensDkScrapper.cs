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
        private String currentUrl = "";

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

            Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/",
                TypesOfProduct.Indoors);

            Start(
                "https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/",
                TypesOfProduct.Outdoors);

            Start(
                "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/",
                TypesOfProduct.Others);
            
            _driver.Quit();
        }

        private void Start(String urlToScrap, Enum type)
        {
            saveProductsAgain:
            _driver?.Quit();
           
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl(urlToScrap);
            _driverItem?.Quit();
          
            Console.WriteLine("Start scrapping: Flugger Horsnes.dk");
               tryMainItem:
                currentUrl = urlToScrap;
                try
                {
                    GetAllItems(urlToScrap, type);

                }
                catch (WebDriverException e)
                {
                    Console.WriteLine(e);
                    goto tryMainItem;
                }
        }


        private void GetAllItems(string urlToScrap, Enum type)
        {
            ReadOnlyCollection<IWebElement> items;
            _driver.Navigate()
                .GoToUrl(urlToScrap);
            Thread.Sleep(3000);
            CleanWindow(_driver);
            IWebElement progressOfItems = null;
            goBackLoading:
            progressOfItems = _driver.FindElement(By.Id("progress_count"));
            IWebElement showMoreButton = null;
            Thread.Sleep(4000);
            showMoreButton = _driver.FindElement(By.Id("loadmore"));

            String previousCount = "";

            while (!AreAllItemsDisplayed(progressOfItems.Text))
            {
                try
                {
                    Thread.Sleep(4000);
                    showMoreButton.Click();
                    Thread.Sleep(4000);
                    progressOfItems = _driver.FindElement(By.Id("progress_count"));
                }
                catch (Exception e)
                {
                    goto goBackLoading;
                }


                if (progressOfItems.Text.Equals(previousCount))
                {
                    break;
                }

                previousCount = progressOfItems.Text;
            }

            Thread.Sleep(4000);
            items = _driver.FindElements(By.ClassName("woocommerce-LoopProduct-link"));

            for (int j = 4; j < items.Count; j++)
            {
                String link = GetLink(items[j]);
                String name = ScrappingHelper.RemoveDiacritics(GetName(items[j]));
                Dictionary<float, int> sizeAndPrice;
                Product product = null;
                String pathToImage;
                tryItemMore:
                try
                {
                    sizeAndPrice = GetSizeAndPrice(link);
                    pathToImage = GetPathToTheImage(link);
                }
                catch (WebDriverException e)
                {
                    Console.WriteLine(e);
                    _driverItem?.Quit();
                    goto tryItemMore;
                }
                _driverItem.Quit();
                foreach (KeyValuePair<float, int> i in sizeAndPrice)
                {
                    product = new Product();
                    product.Name = name;
                    string size = i.Key.ToString();
                    string price = i.Value.ToString();
                    product.Size = size;
                    product.Price = price;
                    product.PathToImage = pathToImage;
                    product.ProductTypeId = Convert.ToInt32(type);
                    product.WebsiteId = 4;
                    Console.WriteLine(product.ToString());
                    ScrappingHelper.SaveOrUpdate(_dbContext, product);
                }

                items = _driver.FindElements(By.ClassName("woocommerce-LoopProduct-link"));
            }
        }


        private String GetPathToTheImage(string link)
        {
            String compoundSrcSet = "";

            Thread.Sleep(2000);
            try
            {
                _driverItem.FindElement(By.Id("main_header"));


                Thread.Sleep(2000);
                IWebElement path = _driverItem.FindElement(By.ClassName("attachment-product-image"));
                compoundSrcSet = path.GetAttribute("src");
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine("Check the path of image exception");
            }

            return compoundSrcSet;
        }


        private Dictionary<float, int> GetSizeAndPrice(String link)
        {
            int counter_color = 0;
            Dictionary<float, int> SizeAndPrice;

            SizeAndPrice = new Dictionary<float, int>();
            _driverItem = new ChromeDriver();
            _driverItem.Navigate()
                .GoToUrl(link);
            Thread.Sleep(2000);
            try
            {
                CleanWindow(_driverItem);
                CleanWindow(_driverItem);
            }
            catch (UnhandledAlertException e)
            {
                IAlert alert = _driverItem.SwitchTo().Alert();
                alert.Accept();
            }

            
            List<IWebElement> colors;
            colors = _driverItem.FindElements(By.ClassName("color-box")).ToList();
            if (colors.Count != 0)
            {
                Console.WriteLine(colors[counter_color].GetAttribute("class"));
                if (!colors[counter_color].GetAttribute("class").Contains("active"))
                {
                    colors = _driverItem.FindElements(By.ClassName("color-box")).ToList();
                    colors[counter_color].Click();
                }
                else if (colors[counter_color].GetAttribute("class").Contains("active") &&
                         colors[counter_color].GetAttribute("class").Contains("inactive"))
                {
                    goBack3:
                    try
                    {
                        clickTwiceTheFirstColor(counter_color);
                        colors = _driverItem.FindElements(By.ClassName("color-box")).ToList();
                        while (colors[counter_color].GetAttribute("class").Contains("active") &&
                               colors[counter_color].GetAttribute("class").Contains("inactive"))
                        {
                            colors = _driverItem.FindElements(By.ClassName("color-box")).ToList();
                            IWebElement color = colors[++counter_color];
                            color.Click();
                            if (color.GetAttribute("id").Contains("color-picker"))
                            {
                                Thread.Sleep(3000);
                                IWebElement listColorWrap =
                                    _driverItem.FindElement(By.ClassName("flugger-list-color"));
                              Thread.Sleep(3000);
                              listColorWrap.Click();
                              IWebElement fluggerColorWrapBig = _driverItem.FindElement(By.ClassName("flugger-color-wrap-big"));
                              IWebElement fluggerListColorBig =
                                  fluggerColorWrapBig.FindElement(By.ClassName("flugger-list-color-big-column"));
                              IWebElement fluggerListColorBigColumnCell =
                                  fluggerListColorBig.FindElement(By.ClassName("flugger-list-color-big-column-cell"));
                              fluggerListColorBigColumnCell.Click();
                              Thread.Sleep(2000);
                              IWebElement colorClosePopup =
                                  _driverItem.FindElement(By.ClassName("select_color_close_popup"));
                              colorClosePopup.Click();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        goto goBack3;
                    }
                }
            }

            goBack:
            List<IWebElement> selectedSize = GetAllSizes();
            String cleanedPrice = "";
            String previous_price = "";
            foreach (IWebElement size in selectedSize)
            {
                Thread.Sleep(4000);
                size.Click();
                Thread.Sleep(8000);
                IWebElement price = _driverItem.FindElement(By.Id("price"));
                try
                {
                    cleanedPrice = CleanPrice(price.FindElement(By.TagName("ins")).Text);
                }
                catch (NoSuchElementException exception)
                {
                    cleanedPrice = CleanPrice(price.FindElement(By.TagName("bdi")).Text);
                }

                if (cleanedPrice.Equals(previous_price))
                {
                    SizeAndPrice.Clear();
                    Thread.Sleep(6000);
                    colors = _driverItem.FindElements(By.ClassName("color-box")).ToList();
                    Thread.Sleep(6000);
                    try
                    {
                        colors[++counter_color].Click();
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine(e + "\nTry again");
                        counter_color = 0;
                    }
                    catch (ElementClickInterceptedException e)
                    {
                        Console.WriteLine(e + "\nTry again");
                        counter_color = 0;
                    }

                    goto goBack;
                }

                previous_price = cleanedPrice;

                String cleanedSize = CleanSize(size.Text);
                try
                {
                    SizeAndPrice.Add(float.Parse(cleanedSize), Int32.Parse(cleanedPrice));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new Dictionary<float, int>();
                }
            }

            return SizeAndPrice;
        }

        private void clickTwiceTheFirstColor(int counter_color)
        {
            List<IWebElement> colors;
            colors = _driverItem.FindElements(By.ClassName("color-box")).ToList();
            try
            {
                colors[counter_color].Click();
                colors[counter_color].Click();
            }
            catch (ElementClickInterceptedException e)
            {
                goBack2:
                try
                {
                    colors = _driverItem.FindElements(By.ClassName("color-box")).ToList();
                    Thread.Sleep(4000);
                    colors[counter_color].Click();
                }
                catch (ElementClickInterceptedException exception)
                {
                    goto goBack2;
                }
            }
        }

        private String CleanSize(String size)
        {
            String cleanedSize = size.Replace(" liter", "").Replace(",", ".").Replace(" stk", "");
            return cleanedSize;
        }

        private String CleanPrice(String price)
        {
            String cleanPrice = price.Split(",")[0].Replace(".", "");
            return cleanPrice;
        }

        private List<IWebElement> GetAllSizes()
        {
            try
            {
                List<IWebElement> variationsWrap =
                    _driverItem.FindElements(By.ClassName("custom-variations__wrap")).ToList();
                if (variationsWrap.Count == 0)
                {
                    return new List<IWebElement>();
                }

                List<IWebElement> amountBoxes;
                List<IWebElement> inactiveBoxes;
                if (variationsWrap.Count > 1)
                {
                    amountBoxes = variationsWrap[1].FindElements(By.ClassName("amount-box")).ToList();
                    inactiveBoxes = variationsWrap[1].FindElements(By.ClassName("inactive")).ToList();
                }
                else
                {
                    amountBoxes = variationsWrap[0].FindElements(By.ClassName("amount-box")).ToList();
                    inactiveBoxes = variationsWrap[0].FindElements(By.ClassName("inactive")).ToList();
                }

                Console.WriteLine(amountBoxes.Count);
                try
                {
                    IWebElement pickerBox = _driverItem.FindElement(By.Id("amount-picker"));
                    amountBoxes.Remove(pickerBox);
                }
                catch (Exception e)
                {
                }

                for (int i = 0; i < inactiveBoxes.Count; i++)
                {
                    amountBoxes.Remove(inactiveBoxes[i]);
                }

                for (int i = 0; i < amountBoxes.Count; i++)
                {
                    IWebElement webElement = amountBoxes[i];
                    String classNames = amountBoxes[i].GetAttribute("class");
                    Thread.Sleep(4000);
                    if (classNames.Contains("active"))
                    {
                        webElement.Click();
                    }
                }

                return amountBoxes;
            }
            catch (NoSuchElementException e)
            {
                return new List<IWebElement>();
            }
        }

        private void CleanWindow(IWebDriver driver)
        {
            try
            {
                IWebElement firstWindow =
                    driver.FindElement(By.Id("CybotCookiebotDialogBody"));

                IJavaScriptExecutor js = (IJavaScriptExecutor) driver;
                js.ExecuteScript("arguments[0].remove()", firstWindow);
                Thread.Sleep(5000);
                js.ExecuteScript(
                    "document.querySelector(`[srcdoc='<!doctype html><html><head></head><body></body></html>']`).contentWindow.document.querySelector('#sleeknote-form div').click()");
                // ReadOnlyCollection<IWebElement> forms = driver.FindElements(By.TagName("form"));
                // js.ExecuteScript("arguments[0].remove()", forms[^1]);
                // ReadOnlyCollection<IWebElement> iframes = driver.FindElements(By.TagName("iframe"));
                // js.ExecuteScript("argument[0].remove()", iframes[0]);
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