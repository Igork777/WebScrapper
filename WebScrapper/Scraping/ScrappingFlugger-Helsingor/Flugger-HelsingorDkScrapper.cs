using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.Enums;
using WebScrapper.Scraping.ScrappingFluggerDk.Repositories;


namespace WebScrapper.Scraping
{
    public class FluggerHelsingorDkScrapper
    {
        private UnitOfWork _unitOfWork;
        private List<Product> _productsIndoor;
        private List<Product> _productsOutdoor;
        private List<Product> _productsTool;
        private List<Product> _productsOther;

        public FluggerHelsingorDkScrapper(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _productsIndoor = new List<Product>();
            _productsOutdoor = new List<Product>();
            _productsTool = new List<Product>();
            _productsOther = new List<Product>();
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.String")]
        public void StartScrapping()
        {
            Console.WriteLine("Starting new scrap");


            //  _unitOfWork.ProductsType.AddRange(ScrappingHelper._allProductTypes);
            //   _unitOfWork.Website.Add(ScrappingHelper._allWebsites[1]);
            _productsIndoor.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/indendoers-maling/loft-og-vaeg/",
                TypesOfProduct.Indoors));
            PopulateProducts(_productsIndoor, ScrappingHelper._allProductTypes[0]);
            _unitOfWork.Complete();

            // _productsIndoor.AddRange(Start(
            //     "https://flugger-helsingor.dk/vare-kategori/indendoers-maling/trae-og-metal/",
            //     TypesOfProduct.Indoors));
            // _productsIndoor.AddRange(Start(
            //     "https://flugger-helsingor.dk/vare-kategori/indendoers-maling/grundere-indendors-maling/",
            //     TypesOfProduct.Indoors));
            //
            // _productsOutdoor.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/udendors-maling/facade/",
            //     TypesOfProduct.Outdoors));
            // _productsOutdoor.AddRange(Start(
            //     "https://flugger-helsingor.dk/vare-kategori/udendors-maling/traebeskyttelse-og-metalmaling/",
            //     TypesOfProduct.Outdoors));
            // _productsOutdoor.AddRange(Start(
            //     "https://flugger-helsingor.dk/vare-kategori/udendors-maling/grundere-udendors-maling/",
            //     TypesOfProduct.Outdoors));
            //
            // _productsOther.AddRange(Start(
            //     "https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvlak-gulvbehandling/",
            //     TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvmaling/",
            //     TypesOfProduct.Others));
            // _productsOther.AddRange(Start(
            //     "https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvolie-gulvbehandling/",
            //     TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/moebelpleje/",
            //     TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/linolier/",
            //     TypesOfProduct.Others));
            // _productsOther.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/bejdse/",
            //     TypesOfProduct.Others));
            //
            // _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/tilbehor/rengoring-og-afdaekning/",
            //     TypesOfProduct.Tools));
            // _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/tilbehor/spartel-og-fuge/",
            //     TypesOfProduct.Tools));
            // _productsTool.AddRange(Start(
            //     "https://flugger-helsingor.dk/vare-kategori/tilbehor/vaegbeklaedning-og-klaebere/",
            //     TypesOfProduct.Tools));
            // _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/bakker-og-spande/",
            //     TypesOfProduct.Tools));
            // _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/malerruller/",
            //     TypesOfProduct.Tools));
            // _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/pensler/",
            //     TypesOfProduct.Tools));
            // _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/spartler-og-skrabere/",
            //     TypesOfProduct.Others));
            // _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/vaegbeklaedning/",
            //     TypesOfProduct.Tools));

            PopulateProducts(_productsOutdoor, ScrappingHelper._allProductTypes[1]);
            PopulateProducts(_productsTool, ScrappingHelper._allProductTypes[2]);
            PopulateProducts(_productsOther, ScrappingHelper._allProductTypes[3]);
        }

        private void PopulateProducts(IList<Product> products, ProductType productType)
        {
            foreach (var product in products)
            {
                product.Website = ScrappingHelper._allWebsites[1];
                product.ProductType = productType;
                _unitOfWork.Products.Add(product);
            }
        }

        private IList<Product> Start(String urlToScrap, Enum type)
        {
            List<Product> products = new List<Product>();
            List<String> pages = GetPages(urlToScrap);
            foreach (String page in pages)
            {
                products.AddRange(GetProducts(page));
            }
            return new List<Product>();
        }

        private List<Product> GetProducts(string page)
        {
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver(options);
            CleanWindows(driver);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            driver.Navigate().GoToUrl(page);
            List<String> link = GetLinks(driver);
            //List<String> names = GetNames(driver);
            // if (link.Count != names.Count)
            // {
            //     throw new RuntimeWrappedException("Something strange happened");
            // }
            for (int i = 0; i < link.Count; i++)
            {
                for (int j = 0; j < GetSizeAndPrice(link[i]).Count; j++)
                {
                    
                }
                Product product = new Product();
              //  product.Name = names[i];
            }
            
            driver.Quit();
            return new List<Product>();
        }

        private Dictionary<String, String> GetSizeAndPrice(String link)
        {
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver(options);
            CleanWindows(driver);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            driver.Navigate().GoToUrl(link);
            try
            {
                List<IWebElement> colorAndSizeSelectors =
                    driver.FindElements(By.ClassName("wc-default-select")).ToList();
                if (colorAndSizeSelectors.Count == 0)
                {
                    IWebElement price = driver.FindElement(By.ClassName("price"));
                    IWebElement pr = price.FindElement(By.TagName("ins"));
                    Console.WriteLine("Text :" + pr.Text);
                }
                else
                {
                    List<IWebElement> colors = colorAndSizeSelectors[0].FindElements(By.ClassName("attached")).ToList();
                    List<IWebElement> size = colorAndSizeSelectors[1].FindElements(By.ClassName("attached")).ToList();
                    
                    colors[0].Click();
                    size[0].Click();
                }

                Console.WriteLine(colorAndSizeSelectors.Count);
            }
            catch (Exception e)
            {

            }

            
            
         //  Console.WriteLine("Color: " + colors[0]);
           return new Dictionary<string, string>();
        }
        

        private List<string> GetLinks(IWebDriver driver)
        {
            HashSet<String> hrefs = new HashSet<string>();
            List<IWebElement> a = driver.FindElements(By.ClassName("woocommerce-LoopProduct-link")).ToList();
            foreach (IWebElement webElement in a)
            {
                hrefs.Add(webElement.GetAttribute("href"));
            }
            return hrefs.ToList();
        }

        private List<String> GetNames(IWebDriver driver)
        {
            List<String> names = new List<string>();
            List<IWebElement> namesElements = driver.FindElements(By.ClassName("woocommerce-loop-product__title")).ToList();
            foreach (IWebElement name in namesElements)
            {
                names.Add(name.Text);
                Console.WriteLine(name.Text);
            }

            return names;

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
                Console.WriteLine("No need to clear the window");
            }
        }
        private List<String> GetPages(String urlToScrap)
        {
            ChromeOptions options = new ChromeOptions();
            IWebDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(urlToScrap);
            CleanWindows(driver);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
            Thread.Sleep(4000);
            List<String> pages = new List<string>();
            List<IWebElement> pagination = driver.FindElements(By.ClassName("page-numbers")).ToList();
            if (pagination.Count > 1)
            {
                pagination.RemoveAt(0);
                pagination.RemoveAt(pagination.Count - 1);
            

                int i = 0;
                pages.Add(urlToScrap);
                foreach (IWebElement page in pagination)
                {
                    if (i != 0)
                    {
                        Console.WriteLine(page.Text);
                        Console.WriteLine(page.GetAttribute("href"));
                        pages.Add(page.GetAttribute("href"));
                    }
                    i++;
                }
            }
            driver.Quit();
            return pages;
        }

        // private List<HtmlNode> GetProductsList(HtmlDocument htmlDocument)
        // {
        //     return htmlDocument.DocumentNode.Descendants("div")
        //         .Where(node => node.GetAttributeValue("class", "").Equals("wcbd_product_details")).ToList();
        // }
        // private List<String> GetPathToImages(HtmlDocument htmlDocument)
        // {
        //     List<String> paths = new List<string>();
        //     List<HtmlNode> webElements = htmlDocument.DocumentNode.Descendants("img").Where(node =>
        //         node.GetAttributeValue("class", "")
        //             .Equals("attachment-woocommerce_thumbnail size-woocommerce_thumbnail")).ToList();
        //     int iterator = 1;
        //     foreach (HtmlNode htmlNode in webElements)
        //     {
        //         if (iterator % 2 != 0)
        //         {
        //             string url = htmlNode.Attributes["src"].Value;
        //             url = ScrappingHelper.FixInvalidCharacter(url, ScrappingHelper.InvalidCharacter);
        //             Console.WriteLine("Path to image: " + url);
        //             paths.Add(url);
        //         }
        //         iterator++;
        //     }
        //     return paths;
        // }
        //
        // [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
        //     MessageId = "type: System.String")]
        // private IList<Product> GetScrappedProducts(List<HtmlDocument> listOfDocuments)
        // {
        //     int iteratorForProxies = 0;
        //      ScrappingHelper.RenewIpAndPorts();
        //      List<HtmlNode> productsList = new List<HtmlNode>();
        //      List<String> pathsToImage = new List<String>();
        //     List<String> hrefs = new List<String>();
        //     List<Product> products = new List<Product>();
        //     foreach (HtmlDocument htmlDocument in listOfDocuments)
        //     {
        //         pathsToImage.AddRange(GetPathToImages(htmlDocument));
        //         productsList.AddRange(GetProductsList(htmlDocument));
        //         if (pathsToImage.Count == productsList.Count)
        //         {
        //             Console.WriteLine("Amazing match");
        //         }
        //
        //         hrefs.AddRange(GetProductsLink(htmlDocument));
        //     }
        //
        //     if (productsList.Count != hrefs.Count)
        //         throw new Exception("Amount of elements in product list and hrefs are not the same");
        //
        //     int i = 0;
        //     tryAnotherIP:
        //     Console.WriteLine("Trying ip: " + ScrappingHelper.proxies[iteratorForProxies]);
        //     try
        //     {
        //         for (; i < productsList.Count; i++)
        //         {
        //             int sizes = GetSizes(hrefs[i],ScrappingHelper.proxies[iteratorForProxies], ScrappingHelper.ports[iteratorForProxies]).Count;
        //             int prices = GetPrices(hrefs[i], sizes, ScrappingHelper.proxies[iteratorForProxies], ScrappingHelper.ports[iteratorForProxies]).Count;
        //             List<String> allSizes = GetSizes(hrefs[i], ScrappingHelper.proxies[iteratorForProxies], ScrappingHelper.ports[iteratorForProxies]);
        //             List<String> allPrices = GetPrices(hrefs[i], sizes, ScrappingHelper.proxies[iteratorForProxies], ScrappingHelper.ports[iteratorForProxies]).ToList();
        //             for (int j = 0; j < prices; j++)
        //             {
        //                 Product tempProduct = new Product();
        //                 tempProduct.Name = GetNameOfProduct(productsList[i]);
        //                 tempProduct.PathToImage = pathsToImage[i];
        //                 Console.WriteLine("Name: " + tempProduct.Name);
        //                 Console.WriteLine("Path to image: " + tempProduct.PathToImage);
        //                 try
        //                 {
        //                     tempProduct.Size = allSizes[j];
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     tempProduct.Size = "No data";
        //                 }
        //
        //                 Console.WriteLine("Size: " + tempProduct.Size);
        //                 tempProduct.Price = allPrices[j];
        //                 Console.WriteLine("Price: " + tempProduct.Price);
        //                 Console.WriteLine("------------------------------------------");
        //                 products.Add(tempProduct);
        //             }
        //
        //             // GetSizes(hrefs[i], proxies[iteratorForProxies], ports[iteratorForProxies]);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ScrappingHelper.proxies[iteratorForProxies] + " failed");
        //         if (iteratorForProxies >= ScrappingHelper.proxies.Count - 1)
        //         {
        //             iteratorForProxies = 0;
        //         }
        //         else
        //         {
        //             iteratorForProxies++;
        //         }
        //
        //         goto tryAnotherIP;
        //     }
        //
        //     return products;
        // }
        //
        //
        //
        //
        // // private IList<string> GetPrices(String url, String proxy, int port)
        // private HashSet<string> GetPrices(string url, int amountOfSizes, String proxy, int port)
        // {
        //     int counter = 0;
        //     Regex correctRowRegex = new Regex("var aepc_wc_add_to_cart =.*");
        //     Regex JSONobjRegex = new Regex("{.*};");
        //     Regex idsRegex = new Regex("\"[0-9]+\":");
        //     Regex valueRegex = new Regex("\"value\":[0-9]+");
        //     // HtmlDocument htmlDocument =
        //     //     ScrappingHelper.GetHtmlDocument(url, proxy, port);
        //     HtmlDocument htmlDocument =
        //         ScrappingHelper.GetHtmlDocument(url);
        //     HashSet<string> correstedValues = null;
        //     try
        //     {
        //         var pricesAsJson = htmlDocument.DocumentNode.Descendants("script").First(node =>
        //             node.GetAttributeValue("id", "").Equals("aepc-pixel-events-js-extra"));
        //         string correctRowMatched = correctRowRegex.Match(pricesAsJson.InnerText).Value;
        //         string preparedJSON = JSONobjRegex.Match(correctRowMatched).Value;
        //         preparedJSON = preparedJSON.Replace(";", "");
        //         StringBuilder JSON = new StringBuilder(preparedJSON);
        //         MatchCollection ids = idsRegex.Matches(JSON.ToString());
        //         MatchCollection values = valueRegex.Matches(JSON.ToString());
        //
        //         correstedValues = new HashSet<string>();
        //         foreach (Match val in values)
        //         {
        //             if (counter > amountOfSizes)
        //             {
        //                 break;
        //             }
        //
        //             correstedValues.Add(val.Value.Replace("\"value\":", ""));
        //             counter++;
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine("Exception occured: " + e);
        //     }
        //
        //
        //     return correstedValues;
        // }
        //
        // // private IList<string> GetSizes(String url, String proxy, int port)
        // [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
        //     MessageId = "type: System.String")]
        // [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
        //     MessageId = "type: System.String")]
        // [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
        //     MessageId = "type: System.String")]
        // private List<string> GetSizes(String url, string proxy, int port)
        // {
        //     // HtmlDocument htmlDocument =
        //     //     ScrappingHelper.GetHtmlDocument(url, proxy, port);
        //     HtmlDocument htmlDocument =
        //         ScrappingHelper.GetHtmlDocument(url);
        //
        //     List<String> sizes = new List<string>();
        //     Regex checkOption = new Regex("[0-9].*");
        //     var select = htmlDocument.DocumentNode.Descendants("select")
        //         .FirstOrDefault(node => node.GetAttributeValue("id", "").Equals("pa_stoerrelse_ml"));
        //     if (select == null)
        //     {
        //         select = htmlDocument.DocumentNode.Descendants("select").FirstOrDefault(node =>
        //             node.GetAttributeValue("id", "").Equals("pa_stoerrelse_mm"));
        //         if (select == null)
        //         {
        //             return new List<string>();
        //         }
        //     }
        //
        //     foreach (var i in select.ChildNodes)
        //     {
        //         if (!checkOption.IsMatch(i.InnerText))
        //         {
        //             continue;
        //         }
        //
        //         String size = i.InnerText.Replace(",", ".");
        //         if (ScrappingHelper.CheckIfInvalidCharacter(i.InnerText, ScrappingHelper.InvalidCharacter))
        //         {
        //             var fixedText = ScrappingHelper.FixInvalidCharacter(size, ScrappingHelper.InvalidCharacter);
        //             sizes.Add(fixedText);
        //         }
        //         else
        //         {
        //             sizes.Add(size);
        //         }
        //     }
        //
        //     return sizes;
        // }
        //
        //
        // private List<String> GetProductsLink(HtmlDocument product)
        // {
        //     IEnumerable<HtmlNode> allLinks = product.DocumentNode.Descendants("a").Where(node =>
        //         node.GetAttributeValue("class", "")
        //             .Equals("woocommerce-LoopProduct-link woocommerce-loop-product__link")).ToList();
        //     List<String> hrefs = new List<string>();
        //     foreach (var link in allLinks)
        //     {
        //         hrefs.Add(link.Attributes["href"].Value);
        //     }
        //
        //     return hrefs;
        // }
        //
        // private String GetNameOfProduct(HtmlNode product)
        // {
        //     String tempName;
        //     tempName = product.Descendants("h2").First().InnerText.Replace("\r\n", "").Trim();
        //     if (!ScrappingHelper.CheckIfInvalidCharacter(tempName, ScrappingHelper.InvalidCharacter))
        //     {
        //         return tempName;
        //     }
        //
        //     return ScrappingHelper.FixInvalidCharacter(tempName, ScrappingHelper.InvalidCharacter);
        // }
        //
        // private IList<String> GetAllPagesLinks(HtmlDocument htmlDocument)
        // {
        //     IList<String> links = new List<string>();
        //     IList<HtmlNode> nodes = htmlDocument.DocumentNode.Descendants("a")
        //         .Where(node => node.GetAttributeValue("class", "").Equals("page-numbers")).ToList();
        //     foreach (var a in nodes)
        //     {
        //         links.Add(a.Attributes["href"].Value);
        //     }
        //
        //     return links;
        // }
    }
}