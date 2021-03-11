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

        public FluggerHorsensDkScrapper(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void StartScrapping()
        {
            Console.WriteLine("Starting new scrap");
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
        }

        private void Start(String urlToScrap, Enum type)
        {
             GetAllItems(urlToScrap, type);
        }

        private void GetAllItems(string urlToScrap, Enum type)
        {
            ScrappingHelper.RenewIpAndPorts();
            int iterator = 0;
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver =  new ChromeDriver(options);
            List<Product> products = new List<Product>();
            Console.WriteLine("Trying IP: " + ScrappingHelper.proxies[iterator]);


            ReadOnlyCollection<IWebElement> items;
            driver.Navigate()
                .GoToUrl(urlToScrap);
            CleanWindow(driver);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            IWebElement progressOfItems = driver.FindElement(By.Id("progress_count"));
            IWebElement showMoreButton = driver.FindElement(By.Id("loadmore"));
          
            String previousCount = "";
            while (!AreAllItemsDisplayed(progressOfItems.Text))
            {
                Thread.Sleep(4000);
                showMoreButton.Click();
                progressOfItems = driver.FindElement(By.Id("progress_count"));
                if (progressOfItems.Text.Equals(previousCount))
                {
                    break;
                }
                previousCount = progressOfItems.Text;
            }


            items = driver.FindElements(By.ClassName("woocommerce-LoopProduct-link"));
            
            Regex priceRegex = new Regex("[1-9]([0-9]{0,2}|\\.)+,");
            for (int j = 0; j < items.Count; j++)
            {
                try
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
                }
                catch (Exception e)
                {
                   continue;
                }
              
            }
            driver.Quit();
        }

        private String GetPathToTheImage(string link)
        {
            Regex pngImage = new Regex("http.*?\\.png");
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver(options);
            driver.Navigate()
                .GoToUrl(link);
            CleanWindow(driver);
            Thread.Sleep(10000);
            IWebElement path = driver.FindElement(By.ClassName("attachment-product-image"));
            String compoundSrcSet = path.GetAttribute("srcset");

            String[] sets = compoundSrcSet.Split(",");
            String finishedString = "No data";
            foreach (String set in sets)
            {
                if (set.Contains(".png"))
                {
                    finishedString = pngImage.Match(set).Value;
                    Console.WriteLine(finishedString);
                    break;
                }
            }
            driver.Quit();
            return finishedString;
        }


        private Dictionary<String, String> GetSizeAndPrice(String link)
        {
            int iterator = 0;
            Dictionary<String, String> SizeAndPrice = new Dictionary<String, string>();
            ChromeOptions options = new ChromeOptions();
            ScrappingHelper.RenewIpAndPorts();
           
            tryAnotherIP:
            Console.WriteLine("Trying IP: " + ScrappingHelper.proxies[iterator]);
            // try
            // {
                // Proxy proxy = new Proxy();
                // proxy.Kind = ProxyKind.Manual;
                // proxy.IsAutoDetect = false;
                // proxy.HttpProxy =
                //     proxy.SslProxy = ScrappingHelper.proxies[iterator] + ":" + ScrappingHelper.ports[iterator];
                // options.Proxy = proxy;
                // options.AddArgument("ignore-certificate-errors");
                using (IWebDriver driver = new ChromeDriver(options))
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor) driver;
                    driver.Navigate()
                        .GoToUrl(link);
                    Thread.Sleep(4000);
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
                    Regex sizeRegex = new Regex("([0-9]{0,2}|,)[0-9]+");
                    List<IWebElement> selectedSize = GetAllSizes(allSizes);
                    Thread.Sleep(4000);
                    List<String> prices = new List<String>();
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

                  
                    //      }
                    // catch (Exception ex)
                    // {
                    //     Console.WriteLine(ScrappingHelper.proxies[iterator] + " failed");
                    //     if (iterator >= ScrappingHelper.proxies.Count - 1)
                    //     {
                    //         iterator = 0;
                    //     }
                    //     else
                    //     {
                    //         iterator++;
                    //     }
                    //
                    //     goto tryAnotherIP;
                    // }

                    driver.Quit();
                    return SizeAndPrice;
                }
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
            String name = product.FindElement(By.ClassName("woocommerce-loop-product__title")).Text;
            name = name.Replace("Flügger ", "");
            return name;
        }


        private String GetLink(IWebElement product)
        {
            return product.GetAttribute("href");
        }
    }
}