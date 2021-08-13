using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using WebJobStarter.DbContext;
using WebJobStarter.DTO;
using WebJobStarter.Enums;
using WebJobStarter.Helpers;

namespace WebJobStarter.ScrappingFluggernaerumDk
{
    public class FluggerNaerumDkScrapper
    {
         private DBContext _dbContext;
        private IWebDriver _driver, _driverItem;

        public FluggerNaerumDkScrapper(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void StartScrapping()
        {
          Start("https://flugger-naerum.dk/k/indendoers-maling/", TypesOfProduct.Indoors);
           Start("https://flugger-naerum.dk/k/udendoers-maling/", TypesOfProduct.Outdoors);
           Start("https://flugger-naerum.dk/k/tilbehoer/", TypesOfProduct.Others);
        }

        private void Start(String urlToScrap, TypesOfProduct type)
        {
            List<String> linksToAllThePages = new List<string>();
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            _driver = new ChromeDriver(chromeOptions);
            _driver.Navigate().GoToUrl(urlToScrap);
            ClearWindow(_driver);
            IWebElement wrapPageNumbers = _driver.FindElement(By.ClassName("page-numbers"));
            List<IWebElement> pageNumbers = wrapPageNumbers.FindElements(By.ClassName("page-numbers")).ToList();
            String link = "";
            for (int i = 0; i < pageNumbers.Count; i++)
            {
                if (pageNumbers[i].GetAttribute("class").Equals("page-numbers current") ||
                    pageNumbers[i].GetAttribute("class").Equals("next page-numbers") ||
                    pageNumbers[i].GetAttribute("class").Equals("prev page-numbers"))
                {
                    continue;
                }

                link = pageNumbers[i].GetAttribute("href");
                linksToAllThePages.Add(link);
            }

            AccessThePage(urlToScrap, true, type);
            for (int i = 0; i < linksToAllThePages.Count; i++)
            {
                AccessThePage(linksToAllThePages[i], false, type);
            }
            _driver.Quit();
        }

        private void AccessThePage(String link, bool isFirstPage, TypesOfProduct type)
        {
            if (!isFirstPage)
            {
                _driver.Navigate().GoToUrl(link);
            }

            IWebElement allProductsWrapper = _driver.FindElement(By.ClassName("products"));
            List<IWebElement> allProducts = allProductsWrapper.FindElements(By.ClassName("product")).ToList();
            for (int i = 0; i < allProducts.Count; i++)
            {
                IWebElement elementLinkAndNameOfProduct =
                    allProducts[i].FindElement(By.ClassName("woocommerce-LoopProduct-link"));
                String linkToProduct = elementLinkAndNameOfProduct.GetAttribute("href");
                List<Product> products = GetProducts(linkToProduct, type);
                for (int j = 0; j < products.Count; j++)
                {
                    Console.WriteLine(products[j]);
                    ScrappingHelper.SaveOrUpdate(_dbContext, products[j]);
                }
            }
            _driver.Navigate().Back();
        }

        private List<Product> GetProducts(string linkToAProduct, TypesOfProduct typesOfProduct)
        {
            List<Product> products = new List<Product>();
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            _driverItem = new ChromeDriver(chromeOptions);
            _driverItem.Navigate().GoToUrl(linkToAProduct);
            ClearWindow(_driverItem);
            Thread.Sleep(2000);
            IWebElement elementProduct = _driverItem.FindElement(By.ClassName("product"));
            IWebElement summaryProduct = elementProduct.FindElement(By.ClassName("summary"));

            Dictionary<double, int> sizesAndPrices = GetSizesAndPrices(summaryProduct);
            foreach (KeyValuePair<double, int> keyValuePair in sizesAndPrices)
            {
                Product product = new Product();
                String pathToImage = GetPathToImage(elementProduct);
                String name = GetName(summaryProduct);
                int typeId = Convert.ToInt32(typesOfProduct);
                int websiteId = 5;
                double size = keyValuePair.Key;
                int price = keyValuePair.Value;
                product.PathToImage = pathToImage;
                product.Name = name;
                product.ProductTypeId = typeId;
                product.WebsiteId = websiteId;
                product.Size = size.ToString();
                product.CurrentPrice = price.ToString();
                products.Add(product);
            }
            _driverItem.Quit();
            return products;
        }

        private void ClearWindow(IWebDriver driver)
        {
            IWebElement cookieWebElement = driver.FindElement(By.Id("cookie_box"));
            IJavaScriptExecutor js = (IJavaScriptExecutor) driver;
            js.ExecuteScript("arguments[0].remove()", cookieWebElement);
        }

        private Dictionary<double, int> GetSizesAndPrices(IWebElement summaryProduct)
        {
            try
            {
                IWebElement form = summaryProduct.FindElement(By.ClassName("variations_form"));
                IWebElement variations = form.FindElement(By.ClassName("variations"));
                List<IWebElement> tableRows = variations.FindElements(By.TagName("tr")).ToList();
                IWebElement amountBoxColorWrap = null;
                IWebElement amountBoxSizeWrap = null;
                bool isFirstColorIsChosen = true;
                bool isFirstSizeBoxIsChosen = true;
                if (tableRows[0].FindElement(By.ClassName("value")).FindElement(By.TagName("select")).GetAttribute("id")
                    .Equals("pa_maengde"))
                {
                    amountBoxSizeWrap = tableRows[0].FindElement(By.ClassName("value"));
                    isFirstSizeBoxIsChosen = checkIfFirstBoxIsChosen(amountBoxSizeWrap);
                    if (tableRows.Count > 1)
                    {
                        amountBoxColorWrap = tableRows[1].FindElement(By.ClassName("value"));
                        isFirstColorIsChosen = checkIfFirstBoxIsChosen(amountBoxColorWrap);
                    }
                }
                else
                {
                    amountBoxColorWrap = tableRows[0].FindElement(By.ClassName("value"));
                    isFirstColorIsChosen = checkIfFirstBoxIsChosen(amountBoxColorWrap);
                    if (tableRows.Count > 1)
                    {
                        amountBoxSizeWrap = tableRows[1].FindElement(By.ClassName("value"));
                        isFirstSizeBoxIsChosen = checkIfFirstBoxIsChosen(amountBoxSizeWrap);
                    }
                }

                if (amountBoxSizeWrap == null)
                {
                    amountBoxSizeWrap = tableRows[0].FindElement(By.ClassName("value"));
                    isFirstSizeBoxIsChosen = checkIfFirstBoxIsChosen(amountBoxColorWrap);
                }

                return GetSizesAndPrices(amountBoxSizeWrap, amountBoxColorWrap, isFirstSizeBoxIsChosen,
                    isFirstColorIsChosen);
                ;
            }
            catch (NoSuchElementException e)
            {
                return new Dictionary<double, int>();
            }
        }

        private Dictionary<double, int> GetSizesAndPrices(IWebElement amountBoxSizeWrap, IWebElement amountBoxColorWrap,
            bool isFirstSizeBoxIsChosen, bool isFirstColorIsChosen)
        {
            Dictionary<double, int> sizeAndPrices = new Dictionary<double, int>();
            String size = "";
            Thread.Sleep(4000);
            if (amountBoxColorWrap != null)
            {
                List<IWebElement> colors = amountBoxColorWrap.FindElements(By.ClassName("select_option_colorpicker"))
                    .ToList();
                if (!isFirstColorIsChosen && colors.Count > 0)
                {
                    colors[0].Click();
                }
            }

            Thread.Sleep(4000);
            List<IWebElement> sizes = amountBoxSizeWrap.FindElements(By.ClassName("select_option_label")).ToList();
            for (int i = 0; i < sizes.Count; i++)
            {
                List<IWebElement> sizes2 = amountBoxSizeWrap.FindElements(By.ClassName("select_option_label")).ToList();
                if (isFirstSizeBoxIsChosen && i == 0)
                {
                    size = sizes2[i].Text;
                    Thread.Sleep(4000);
                    if (size.Contains("x"))
                    {
                        return sizeAndPrices;
                    }
                    KeyValuePair<double, int> sizeAndPrice = GetOneSizeToPrice(size);
                    sizeAndPrices.Add(sizeAndPrice.Key, sizeAndPrice.Value);
                    continue;
                }
                Thread.Sleep(8000);
                size = sizes2[i].Text;
                sizes2 = amountBoxSizeWrap.FindElements(By.ClassName("select_option_label")).ToList();
                Thread.Sleep(8000);
                try
                {
                    sizes2[i].Click();
                }
                catch (Exception e)
                {
                    IWebElement element = amountBoxSizeWrap.FindElements(By.ClassName("select_option_label")).ToList()[i];

                    Actions actions = new Actions(_driverItem);

                    actions.MoveToElement(element).Click().Perform();
                }
                Thread.Sleep(4000);
                if (size.Contains("x"))
                {
                    return sizeAndPrices;
                }

                KeyValuePair<double, int> sizeAndPrice2 = GetOneSizeToPrice(size);
                sizeAndPrices.Add(sizeAndPrice2.Key, sizeAndPrice2.Value);
            }

            return sizeAndPrices;
        }

        private KeyValuePair<double, int> GetOneSizeToPrice(String rawSize)
        {
            double size = GetSize(rawSize);
            IWebElement elementProduct = _driverItem.FindElement(By.ClassName("product"));
            IWebElement summaryProduct = elementProduct.FindElement(By.ClassName("summary"));
            IWebElement form = summaryProduct.FindElement(By.ClassName("variations_form"));
            IWebElement singleVariation = form.FindElement(By.ClassName("single_variation_wrap"));
            IWebElement priceVar = singleVariation.FindElement(By.ClassName("woocommerce-variation-price"));

            IWebElement currentPrice = null;
            try
            {
                 currentPrice = priceVar.FindElement(By.TagName("ins"));
            }
            catch (NoSuchElementException e)
            {
                IWebElement priceWrapper = summaryProduct.FindElement(By.ClassName("price"));
                currentPrice = priceWrapper.FindElement(By.TagName("ins"));
                Console.WriteLine(currentPrice.Text);
            }

             String rawCurrentPrice = currentPrice.Text.Split(",")[0].Replace(".","");
             int price = Int32.Parse(rawCurrentPrice);
            return new KeyValuePair<double, int>(size, price);
        }

        private double GetSize(String rawSize)
        {
            rawSize = rawSize.Replace(",", ".").Replace(" ", "").Replace("liter", "").Replace("Liter", "").Replace("mm", "").Replace("cm", "");
            double size = Double.Parse(rawSize);
            return size;
        }


        private bool checkIfFirstBoxIsChosen(IWebElement sizesWrap)
        {
            List<IWebElement> options = sizesWrap.FindElements(By.ClassName("select_option")).ToList();
            if (options[0].GetAttribute("class").Contains("select_option selected"))
            {
                return true;
            }

            return false;
        }

        private String GetName(IWebElement summaryProduct)
        {
            String name = summaryProduct.FindElement(By.ClassName("product_title")).Text;
            return name;
        }

        private String GetPathToImage(IWebElement elementProduct)
        {
            IWebElement productGallery = elementProduct.FindElement(By.ClassName("woocommerce-product-gallery"));
            IWebElement figureProduct =
                productGallery.FindElement(By.ClassName("woocommerce-product-gallery__wrapper"));
            IWebElement pathToImage = figureProduct.FindElement(By.TagName("a"));
            Thread.Sleep(4000);
            String path = pathToImage.GetAttribute("href");
            return path;
        }
    }
}