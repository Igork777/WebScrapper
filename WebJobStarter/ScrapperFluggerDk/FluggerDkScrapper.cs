using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using WebJobStarter.DbContext;
using WebJobStarter.DTO;
using WebJobStarter.Enums;
using WebJobStarter.Helpers;

namespace WebJobStarter.ScrapperFluggerDk
{
    public class FluggerDkScrapper
    {
        private DBContext _dbContext;
        private IWebDriver _driver;
        private ChromeOptions _options;

        public FluggerDkScrapper(DBContext dbContext)
        {
            _dbContext = dbContext;
            _options = ScrappingHelper.getOptions();
        }

        public void StartScrapping()
        {
            Console.WriteLine("Starting new scrap: Flugger.dk");
            _driver = new RemoteWebDriver(new Uri("https://chrome.browserless.io/webdriver"), _options.ToCapabilities());
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Start("https://www.flugger.dk/maling-tapet/indend%C3%B8rs/", TypesOfProduct.Indoors);
            Start("https://www.flugger.dk/maling-tapet/udend%C3%B8rs/", TypesOfProduct.Outdoors);
            Start("https://www.flugger.dk/maling-tapet/dekoration/", TypesOfProduct.Others);
            Start("https://www.flugger.dk/maling-tapet/tapetkl%C3%A6ber/", TypesOfProduct.Others);
            _driver.Quit();


            Console.WriteLine("Scrap is finished!!!");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        private void Start(String urlToScrap, Enum type)
        {
            saveProductsAgain:
            _driver.Navigate().GoToUrl(urlToScrap);
            try
            {
               
              //  Thread.Sleep(2000);
                ClearTheWindow();
                IWebElement product_list_wrapper = _driver.FindElement(By.ClassName("products-grid-list"));

                List<IWebElement> product_list =
                    product_list_wrapper.FindElements(By.ClassName("products-grid-list-item")).ToList();
                for (int i = 0; i < product_list.Count; i++)
                {
                    product_list_wrapper = _driver.FindElement(By.ClassName("products-grid-list"));
                    List<IWebElement> productList2 =
                        product_list_wrapper.FindElements(By.ClassName("products-grid-list-item")).ToList();
                    String href = productList2[i].FindElement(By.ClassName("product-blocklinkwrap"))
                        .GetAttribute("href");
                    List<Product> products = getProducts(href, type);
                    foreach (Product product in products)
                    {
                        Console.WriteLine(product.ToString());
                        ScrappingHelper.SaveOrUpdate(_dbContext, product);
                    }

                    _driver.Navigate().Back();
                }
            }
            catch (WebDriverException e)
            {
                _driver = new RemoteWebDriver(new Uri("https://chrome.browserless.io/webdriver"), _options.ToCapabilities());
                goto saveProductsAgain;
            }
        }

        private void ClearTheWindow()
        {
            try
            {
                IWebElement coockie_window = _driver.FindElement(By.Id("coiPage-1"));
                IWebElement coockieFooter = coockie_window.FindElement(By.ClassName("coi-banner__page-footer"));
                IWebElement coockieButtonGroup = coockieFooter.FindElement(By.ClassName("coi-button-group"));
                List<IWebElement> buttons_coockie = coockieButtonGroup.FindElements(By.TagName("button")).ToList();
                buttons_coockie[0].Click();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception appeared while clearing the window");
            }
            
            //Thread.Sleep(2000);
            IJavaScriptExecutor js = (IJavaScriptExecutor) _driver;
            try
            {
                js.ExecuteScript(
                    "document.querySelector(`[srcdoc='<!doctype html><html><head></head><body></body></html>']`).contentWindow.document.querySelector('#sleeknote-form div').click()");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private List<Product> getProducts(String href, Enum type)
        {
            _driver.Navigate().GoToUrl(href);
            String productName = getProductName();
            String pathToImage = getPathToImage();
            List<Product> products = new List<Product>();
            try
            {
                Dictionary<float, int> sizeAndPrice = getSizeAndPrice();
                foreach (KeyValuePair<float, int> pair in sizeAndPrice)
                {
                    Product finalProdut = new Product();
                    finalProdut.PathToImage = pathToImage;
                    finalProdut.Size = pair.Key.ToString();
                    finalProdut.CurrentPrice = pair.Value.ToString();
                    finalProdut.WebsiteId = 1;
                    finalProdut.ProductTypeId = Convert.ToInt32(type);
                    if (!ScrappingHelper.CheckIfInvalidCharacter(productName, ScrappingHelper.InvalidCharacter))
                    {
                        finalProdut.Name = productName;
                    }
                    else
                    {
                        finalProdut.Name =
                            ScrappingHelper.FixInvalidCharacter(productName, ScrappingHelper.InvalidCharacter);
                    }

                    products.Add(finalProdut);
                }

                return products;
            }
            catch (Exception e)
            {
                return new List<Product>();
            }
        }

        private Dictionary<float, int> getSizeAndPrice()
        {
            Dictionary<float, int> sizesAndPrices = new Dictionary<float, int>();
            IWebElement productPathToImageWrapper = _driver.FindElement(By.ClassName("product-page-primary"));
            try
            {
                IWebElement productBasket =
                    productPathToImageWrapper.FindElement(By.ClassName("js-product-basket-multiple-wrap"));
                List<IWebElement> product_Sizes_Prices =
                    productBasket.FindElements(By.ClassName("js-product-basket-wrap")).ToList();
                for (int i = 0; i < product_Sizes_Prices.Count; i++)
                {
                    KeyValuePair<float, int> keyValuePair = getSizePricePair(product_Sizes_Prices[i].Text);
                    try
                    {
                        sizesAndPrices.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }

                return sizesAndPrices;
            }
            catch (Exception e)
            {
                IWebElement customColorsList =
                    productPathToImageWrapper.FindElement(By.ClassName("popular-colors-list-item-color"));
                customColorsList.Click();
               // Thread.Sleep(2000);
                IWebElement modalBody = _driver.FindElement(By.ClassName("modal-body"));
                IWebElement js_color_picker = modalBody.FindElement(By.ClassName("js-color-picker-color"));
                js_color_picker.Click();
                //Thread.Sleep(2000);
                IWebElement productVariantsContent = _driver.FindElement(By.ClassName("product-variants-content"));
                IWebElement productBasket =
                    productVariantsContent.FindElement(By.ClassName("js-product-basket-multiple-wrap"));
                List<IWebElement> product_Sizes_Prices =
                    productBasket.FindElements(By.ClassName("js-product-basket-wrap")).ToList();
                for (int i = 0; i < product_Sizes_Prices.Count; i++)
                {
                    KeyValuePair<float, int> keyValuePair = getSizePricePair(product_Sizes_Prices[i].Text);
                    sizesAndPrices.Add(keyValuePair.Key, keyValuePair.Value);
                }

                return sizesAndPrices;
            }
        }

        private KeyValuePair<float, int> getSizePricePair(String productSizesPrices)
        {
            String[] sizeAndPrice = productSizesPrices.Split("\n");
            sizeAndPrice[0] = sizeAndPrice[0].Replace("\r", "").Replace(" L", "").Replace(",", ".").Replace("\n", "");
            sizeAndPrice[1] = sizeAndPrice[1].Split(",")[0].Replace(".", "");
            float size = float.Parse(sizeAndPrice[0]);
            int price = Int32.Parse(sizeAndPrice[1]);
            KeyValuePair<float, int> keyValuePair = new KeyValuePair<float, int>(size, price);
            return keyValuePair;
        }

        private String getProductName()
        {
            IWebElement productNameWrapper = _driver.FindElement(By.ClassName("product-page-header"));
            IWebElement productName = productNameWrapper.FindElement(By.TagName("h1"));
            return productName.Text;
        }

        private String getPathToImage()
        {
            IWebElement productPathToImageWrapper = _driver.FindElement(By.ClassName("product-page-primary"));
            IWebElement productPathToImage = productPathToImageWrapper.FindElement(By.TagName("img"));
            return productPathToImage.GetAttribute("src");
        }
    }
}