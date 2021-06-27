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
            Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/vaegmaling/",
                TypesOfProduct.Indoors);

            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/traemaling-indendoers/",
            //     TypesOfProduct.Indoors);
            //
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/sproejtemaling/",
            //     TypesOfProduct.Indoors);
            //
            // Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/loftmaling/",
            //     TypesOfProduct.Indoors);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/malerruller/", TypesOfProduct.Tools);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/rulleskafter/",
            //     TypesOfProduct.Tools);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/malerspande/", TypesOfProduct.Tools);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/malerbakker/", TypesOfProduct.Tools);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/slibevaertoej/",
            //     TypesOfProduct.Tools);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/rengoering/arbejdshandsker/",
            //     TypesOfProduct.Tools);
            // Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/spartler/",
            //     TypesOfProduct.Tools);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/vaerktoej/fugepistoler/",
            //     TypesOfProduct.Tools);
            //
            //
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/indendoers-grunder/",
            //     TypesOfProduct.Indoors);
            //
            // Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/sandmaling/",
            //     TypesOfProduct.Indoors);
            //
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/indfarvet-spartelmasse-indendoers-maling/",
            //     TypesOfProduct.Indoors);
            //
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/metalmaling/", TypesOfProduct.Indoors);
            //
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/vaegbeklaedning/",
            //     TypesOfProduct.Indoors);
            //
            // Start("https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/gulvmaling/",
            //     TypesOfProduct.Indoors);
            //
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/indendoers-maling/radiatormaling/",
            //     TypesOfProduct.Indoors);
            //
            // Start("https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/",
            //     TypesOfProduct.Outdoors);
            //
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/udendoers-grunder/",
            //     TypesOfProduct.Outdoors);
            //
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/traemaling-udendoers/",
            //     TypesOfProduct.Outdoors);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/udendoers-maling/vinduesmaling/",
            //     TypesOfProduct.Outdoors);
            //
            //
            // Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/kit/",
            //     TypesOfProduct.Others);
            // Start("https://www.flugger-horsens.dk/vare-kategori/tilbehoer/kit/linoliekit/",
            //     TypesOfProduct.Others);
            //
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/silikonefugemasse/",
            //     TypesOfProduct.Others);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/acrylfugemasse/",
            //     TypesOfProduct.Others);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/",
            //     TypesOfProduct.Others);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/fugemasse/indendoers-fugemasse/vaadrumsfugemasse/",
            //     TypesOfProduct.Others);
            //
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningsfilt/",
            //     TypesOfProduct.Others);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningstape/",
            //     TypesOfProduct.Others);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningspap/",
            //     TypesOfProduct.Others);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/afdaekningsplast/",
            //     TypesOfProduct.Others);
            // Start(
            //     "https://www.flugger-horsens.dk/vare-kategori/tilbehoer/afdaekning/udendoers-afdaekningspap/",
            //     TypesOfProduct.Others);
            _driver.Quit();
        }

        private void Start(String urlToScrap, Enum type)
        {
            currentUrl = urlToScrap;
            GetAllItems(urlToScrap, type);
        }


        private void GetAllItems(string urlToScrap, Enum type)
        {
            ReadOnlyCollection<IWebElement> items;
            _driver.Navigate()
                .GoToUrl(urlToScrap);
            Thread.Sleep(3000);
            CleanWindow();
            IWebElement progressOfItems = null;
            progressOfItems = _driver.FindElement(By.Id("progress_count"));
            IWebElement showMoreButton = null;
            Thread.Sleep(4000);
            showMoreButton = _driver.FindElement(By.Id("loadmore"));

            String previousCount = "";
            Thread.Sleep(4000);
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


            for (int j = 0; j < items.Count; j++)
            {
                items = _driver.FindElements(By.ClassName("woocommerce-LoopProduct-link"));
                String link = GetLink(items[j]);
                String name = ScrappingHelper.RemoveDiacritics(GetName(items[j]));


                Console.WriteLine("Name: " + name);
                Dictionary<String, String> sizeAndPrice = GetSizeAndPrice(link);
                Product product = null;
                String pathToImage = GetPathToTheImage(link);
                foreach (KeyValuePair<String, String> i in sizeAndPrice)
                {
                    product = new Product();
                    product.Name = name;
                    string size = i.Key;
                    string price = i.Value;
                    product.Size = size;
                    product.Price = price;
                    product.PathToImage = pathToImage;
                    product.ProductTypeId = Convert.ToInt32(type);
                    product.WebsiteId = 4;
                    Console.WriteLine(product.ToString());
                    ScrappingHelper.SaveOrUpdate(_dbContext, product);
                }
            }
        }


        private String GetPathToTheImage(string link)
        {
            String compoundSrcSet = "";

            Thread.Sleep(2000);
            try
            {
                _driver.FindElement(By.Id("main_header"));


                Thread.Sleep(2000);
                IWebElement path = _driver.FindElement(By.ClassName("attachment-product-image"));
                compoundSrcSet = path.GetAttribute("src");
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine("Check the path of image exception");
            }


            _driver.Navigate().GoToUrl(currentUrl);
            return compoundSrcSet;
        }


        private Dictionary<String, String> GetSizeAndPrice(String link)
        {
            int counter_color = 0;
            Dictionary<String, String> SizeAndPrice;

            SizeAndPrice = new Dictionary<String, string>();

            _driver.Navigate()
                .GoToUrl(link);
            Thread.Sleep(2000);
            CleanWindow();
            _driver.FindElement(By.Id("main_header"));
            IWebElement color = _driver.FindElement(By.ClassName("color-box"));
           
            Thread.Sleep(4000);
            if (!color.GetAttribute("class").Contains("active"))
            {
                List<IWebElement> colors = _driver.FindElements(By.ClassName("color-box")).ToList();
                colors[counter_color].Click();
            }
            else
            {
                List<IWebElement> colors = _driver.FindElements(By.ClassName("color-box")).ToList();
                colors[counter_color].Click();
                Thread.Sleep(4000);
                colors[counter_color].Click();
            }

            Thread.Sleep(2000);
            List<IWebElement> selectedSize = GetAllSizes();
            Thread.Sleep(2000);
            goBack:
            String previous_price = "";
            foreach (IWebElement size in selectedSize)
            {
                size.Click();
                Thread.Sleep(4000);
                IWebElement price = _driver.FindElement(By.Id("price"));
                String cleanedPrice = CleanPrice(price.FindElement(By.TagName("ins")).Text);
                if (cleanedPrice.Equals(previous_price))
                {
                    SizeAndPrice.Clear();
                    List<IWebElement> colors = _driver.FindElements(By.ClassName("color-box")).ToList();
                    colors[++counter_color].Click();
                    goto goBack;
                }
                previous_price = cleanedPrice; 

                String cleanedSize = CleanSize(size.Text);
                SizeAndPrice.Add(cleanedSize, cleanedPrice);
            }

            return SizeAndPrice;
        }

        private String CleanSize(String size)
        {
            String cleanedSize = size.Replace(" liter", "").Replace(",", ".");
            return cleanedSize;
        }

        private String CleanPrice(String price)
        {
            String cleanPrice = price.Split(",")[0].Replace(".", "");
            return cleanPrice;
        }

        private List<IWebElement> GetAllSizes()
        {
            List<IWebElement> amountBoxes = _driver.FindElements(By.ClassName("amount-box")).ToList();
            Console.WriteLine(amountBoxes.Count);
            List<IWebElement> inactiveBoxes = _driver.FindElements(By.ClassName("inactive")).ToList();
            IWebElement pickerBox = _driver.FindElement(By.Id("amount-picker"));


            amountBoxes.Remove(pickerBox);
            for (int i = 0; i < inactiveBoxes.Count; i++)
            {
                amountBoxes.Remove(inactiveBoxes[i]);
            }

            for (int i = 0; i < amountBoxes.Count; i++)
            {
                IWebElement webElement = amountBoxes[i];
                String classNames = amountBoxes[i].GetAttribute("class");
                if (classNames.Contains("active"))
                {
                    Thread.Sleep(2000);
                    webElement.Click();
                }
            }

            return amountBoxes;
        }

        private void CleanWindow2()
        {
        }

        private void CleanWindow()
        {
            try
            {
                IWebElement firstWindow =
                    _driver.FindElement(By.Id("CybotCookiebotDialogBody"));

                IJavaScriptExecutor js = (IJavaScriptExecutor) _driver;
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