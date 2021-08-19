using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using WebJobStarter.DbContext;
using WebJobStarter.DTO;
using WebJobStarter.Enums;
using WebJobStarter.Helpers;

namespace WebJobStarter.ScrappingMaligHalvprisDk
{
    public class Maling_halvprisDkScrapper
    {
          private DBContext _dbContext;
        private IWebDriver _driver;
        private ChromeOptions options;

        
        public Maling_halvprisDkScrapper(DBContext dbContext)
        {
            _dbContext = dbContext;
            options = new ChromeOptions();
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
            //options.AddArgument("--no-sandbox");
            options.AddAdditionalCapability("browserless.token", "4df70c47-9938-437f-aef0-6ea89533a03c", true);
        }

        public void StartScrapping()
        {
            Console.WriteLine("Starting new scrap: Maling.dk");
            _driver = new RemoteWebDriver(new Uri("https://chrome.browserless.io/webdriver"), options.ToCapabilities());
            Start("https://www.maling-halvpris.dk/butik-kob-maling/ral-tex/ral-tex_inde/", TypesOfProduct.Indoors);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/ral-tex/ral-tex_ude/", TypesOfProduct.Outdoors);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/beckers-ude/", TypesOfProduct.Outdoors);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/facademaling/", TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/beckers-inde/", TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/panel-og-traemaling/",
                TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/gulvmaling-fra-beckers/",
                TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/glasfilt/", TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/gori/afvask-og-algebehandling/",
                TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/gori/gori-professionel/", TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/gori/gori-daekkende/", TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/dyrup-inde/", TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/iso-paint/", TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/junckers/", TypesOfProduct.Others);

            Start("https://www.maling-halvpris.dk/butik-kob-maling/terrasseolie/", TypesOfProduct.Outdoors);

            Start("https://www.maling-halvpris.dk/butik-kob-maling/flugger/flugger-ude/", TypesOfProduct.Outdoors);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/flugger/flugger-inde/", TypesOfProduct.Indoors);
            _driver.Quit();
        }

        private void Start(String urlToScrap, Enum type)
        {
            saveProductsAgain:
           // _driver?.Quit();
           _driver.Navigate().GoToUrl(urlToScrap);

            try
            {
                GetProductsList(type);
            }
            catch (WebDriverException e)
            {
                Console.WriteLine(e);
               goto saveProductsAgain;
            }
        }


        private void GetProductsList(Enum type)
        {
            IWebElement allProducts = _driver.FindElement(By.ClassName("products"));
            List<IWebElement> productList = allProducts.FindElements(By.ClassName("product")).ToList();
            for (int i = 0; i < productList.Count; i++)
            {

                String pathToTheImage = productList[i].FindElement(By.TagName("img")).GetAttribute("src");
                String nameOfTheProduct = productList[i].FindElement(By.ClassName("woocommerce-loop-product__title")).Text;
                Dictionary<double, int> sizesAndPrices = GetAllSizesAndPrices(productList[i]);
                SaveProducts(pathToTheImage, nameOfTheProduct, sizesAndPrices, type);
            }
        }

        private void SaveProducts(String pathToImage, String nameOfTheProduct, Dictionary<double, int> sizesAndPrices, Enum type)
        {
            foreach (KeyValuePair<double, int> iterator in sizesAndPrices)
            {
                Product finalProduct = new Product()
                {
                    Name = nameOfTheProduct, CurrentPrice = iterator.Value.ToString(), Size = iterator.Key.ToString(), WebsiteId = 3,
                    ProductTypeId = Convert.ToInt32(type), PathToImage = pathToImage
                };
                if (ScrappingHelper.CheckIfInvalidCharacter(finalProduct.Name, ScrappingHelper.InvalidCharacter))
                {
                    finalProduct.Name =
                        ScrappingHelper.FixInvalidCharacter(finalProduct.Name, ScrappingHelper.InvalidCharacter);
                }
        
                finalProduct.Name = ChangeNameToNormal(finalProduct.Name);
                finalProduct.Name = ScrappingHelper.RemoveDiacritics(finalProduct.Name.Trim());
                if (finalProduct.Name.Equals(""))
                {
                    return;
                }

                Console.WriteLine(finalProduct.ToString());

                ScrappingHelper.SaveOrUpdate(_dbContext, finalProduct);
            }
        }

        private Dictionary<double, int> GetAllSizesAndPrices(IWebElement product)
        {
            try
            {
                IWebElement form = product.FindElement(By.ClassName("variations_form"));
                IWebElement variations = form.FindElement(By.ClassName("variations"));
                List<IWebElement> tableRow = variations.FindElements(By.TagName("tr")).ToList();
                if (tableRow.Count == 1)
                {
                    if (tableRow[0].FindElement(By.ClassName("label")).Text.Contains("Stør"))
                    {
                        return RetirvieCorespondedSizesAndPrices(tableRow[0], form, product);
                    }
                }
                else if (tableRow.Count > 1)
                {
                    IWebElement sizeTableRow = null;
                    for (int i = 0; i < tableRow.Count; i++)
                    {
                        List<IWebElement> tableData = tableRow[i].FindElements(By.ClassName("label")).ToList();
                        if (tableData.Count > 0 && tableData[0].Text.Contains("Farve")|| tableData[0].Text.Contains("Glans") || tableData[0].Text.Contains("Korn"))
                        {
                            IWebElement farversDataValue = tableRow[i].FindElement(By.ClassName("value"));
                            List<IWebElement> options = farversDataValue.FindElements(By.ClassName("attached")).ToList();
                            if (options.Count > 0)
                            {
                                options[0].Click();
                            }
                        }
                        else if (tableData.Count > 0 && tableData[0].Text.Contains("Stør")||tableData[0].Text.Contains("Stær"))
                        {
                            sizeTableRow = tableRow[i];
                        }
                    }

                    return RetirvieCorespondedSizesAndPrices(sizeTableRow, form, product);
                }

                return new Dictionary<double, int>();
            }
            catch (Exception e)
            {
                return new Dictionary<double, int>();
            }
        }

        private Dictionary<double, int> RetirvieCorespondedSizesAndPrices(IWebElement tableRow, IWebElement form, IWebElement supplementaryForm)
        {
            Dictionary<double, int> sizeAndPrice = new Dictionary<double, int>();
            string strSize = "";
            if (tableRow == null)
            {
                return new Dictionary<double, int>();
            }
            IWebElement tableDataValue = tableRow.FindElement(By.ClassName("value"));
            List<IWebElement> options = tableDataValue.FindElements(By.ClassName("attached")).ToList();
            for (int i = 0; i < options.Count; i++)
            {
                List<IWebElement> defineOptionsOnceAgainToAvoidStaleException = DefineOptionsAgain(tableRow);
                strSize = defineOptionsOnceAgainToAvoidStaleException[i].Text;
                defineOptionsOnceAgainToAvoidStaleException[i].Click();
                double cleanedSize = CleanSize(strSize);
                if (cleanedSize == -1)
                {
                    return new Dictionary<double, int>();
                }
                
                int cleanedPrice = GetPrice(form, supplementaryForm);
                sizeAndPrice.Add(cleanedSize, cleanedPrice);
            }

            return sizeAndPrice;
        }

        private List<IWebElement> DefineOptionsAgain(IWebElement tableDataValue)
        {
            return tableDataValue.FindElements(By.ClassName("attached")).ToList();
        }

        private int GetPrice(IWebElement form, IWebElement supplementaryForm)
        {
            IWebElement priceWrapper = form.FindElement(By.ClassName("single_variation_wrap"));
            try
            {
                IWebElement currentPrice = priceWrapper.FindElement(By.TagName("ins"));
               // IWebElement exactPrice = currentPrice.FindElement(By.ClassName("woocommerce-CurrentPrice-amount"));
                return CleanPrice(currentPrice.Text);
            }
            catch (NoSuchElementException exception)
            {
                try
                {
                    IWebElement currentPrice = supplementaryForm.FindElement(By.ClassName("price"));
                    IWebElement exactPrice = currentPrice.FindElement(By.TagName("ins"));
                    return CleanPrice(exactPrice.Text);
                }
                catch (NoSuchElementException ex)
                {
                    try
                    {
                        IWebElement exactPrice = priceWrapper.FindElement(By.ClassName("price"));
                        Console.WriteLine(exactPrice.Text);
                        return CleanPrice(exactPrice.Text);
                    }
                    catch (NoSuchElementException e)
                    {
                        IWebElement exactPrice = supplementaryForm.FindElement(By.ClassName("price"));
                        Console.WriteLine(exactPrice.Text);
                        return CleanPrice(exactPrice.Text);
                    }
                    
                }

                
                
            }
            
        }


        private int CleanPrice(String uncleanedPrices)
        {
            String cleanedPrice = uncleanedPrices.Replace("DKK ", "").Replace(",00", "").Replace(".","").Replace(" ","").Replace("kr","");
            return Int32.Parse(cleanedPrice);
        }

        private double CleanSize(String uncleanedSize)
        {
            String temp = "";
            double size = -1;
            temp = uncleanedSize.Replace("l","").Replace("L", "").Replace(",", ".").Replace(" ", "").Replace("liter", "");
            try
            {
                 size = Double.Parse(temp);
            }
            catch (Exception e)
            {
            }
            return size;
        }
        private String ChangeNameToNormal(string productName)
        {
            String rightProductName = "";
            Regex tilRegex = new Regex("^(.*?) til ");
            Regex characterRegex = new Regex("^(.*?)([!$%^&*()_+|~=`{}\\[\\]:\";'<>?,.\\/])");
            Regex defisRegex = new Regex("^(.*?) – ");
            Match matchCharacter = characterRegex.Match(productName);
            Match matchTitle = tilRegex.Match(productName);
            if (matchCharacter.Success)
            {
                rightProductName = matchCharacter.Value;
                rightProductName = rightProductName.Remove(rightProductName.Length - 1);
                matchTitle = tilRegex.Match(rightProductName);

                if (matchTitle.Success)
                {
                    rightProductName = matchTitle.Value;
                    for (int i = 0; i < 4; i++)
                    {
                        rightProductName = rightProductName.Remove(rightProductName.Length - 1);
                    }
                }
            }
            else if (matchTitle.Success)
            {
                rightProductName = matchTitle.Value;
                for (int i = 0; i < 4; i++)
                {
                    rightProductName = rightProductName.Remove(rightProductName.Length - 1);
                }
            }
            else
            {
                rightProductName = productName;
            }

            Match defis = defisRegex.Match(rightProductName);
            if (defis.Success)
            {
                rightProductName = defis.Value;
                for (int i = 0; i < 3; i++)
                {
                    rightProductName = rightProductName.Remove(rightProductName.Length - 1);
                }
            }
            rightProductName = rightProductName.Replace("Flügger", "");
            rightProductName = rightProductName.Trim();
            rightProductName = ReturnNameWhichIsUsedInOtherTables(rightProductName);
            if (rightProductName.Equals(""))
            {
                return productName;
            }
            return rightProductName;
        }

        private String ReturnNameWhichIsUsedInOtherTables(String nameToCorrect)
        {
            String temp = nameToCorrect;

            List<String> separatedStrings = nameToCorrect.Split(" ").ToList();
            for (int i = separatedStrings.Count - 1; i > 0; i--)
            {
                List<Product> products = _dbContext.Product.Where(product => product.WebsiteId != 3).ToList()
                    .Where(product => product.Name == nameToCorrect).ToList();
                if (products.Count > 0)
                {
                    temp = nameToCorrect.Trim();
                    return temp;
                }

                nameToCorrect = nameToCorrect.Replace(separatedStrings[i], "");
                nameToCorrect = nameToCorrect.Trim();
            }

            return temp;
        }
    }
}