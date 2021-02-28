using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Scraping.ScrappingFluggerDk.Repositories;
using Type = WebScrapper.Scraping.ScrappingFluggerDk.Enums.Type;

namespace WebScrapper.Scraping.ScrappingFluggerDk
{
    public class ProductsScrapper
    {
        private List<Product> _productsIndoor;
        private List<Product> _productsOutdoor;
        private List<Product> _productsTool;
        private List<Product> _productsOther;
        private List<ProductType> _allProductTypes;
        private Website _website;
        private UnitOfWork _unitOfWork;

        public ProductsScrapper()
        {
            _productsIndoor = new List<Product>();
            _productsOutdoor = new List<Product>();
            _productsTool = new List<Product>();
            _productsOther = new List<Product>();
            _allProductTypes = new List<ProductType>()
            {
                new ProductType() {ProductTypeId = 1, Type = "Indoor"},
                new ProductType() {ProductTypeId = 2, Type = "Outdoor"},
                new ProductType() {ProductTypeId = 3, Type = "Tool"},
                new ProductType() {ProductTypeId = 3, Type = "Other"}
            };
            _website = new Website();
            _website.WebsiteId = 1;
            _website.Name = "flugger.dk";

        }

        public void StartScrapping()
        { 
            _unitOfWork = new UnitOfWork(new DBContext());
            Console.WriteLine("Starting new scrap");

            try
            {
                _productsTool.AddRange(Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/pensler-ruller/",
                    Type.Tools));
                _productsIndoor.AddRange(Start("https://www.flugger.dk/maling-tapet/indend%C3%B8rs/", Type.Indoors));
                _productsTool.AddRange(Start(
                    "https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/tapetv%C3%A6rkt%C3%B8j-kl%C3%A6ber/", Type.Tools));
                _productsOutdoor.AddRange(Start("https://www.flugger.dk/maling-tapet/udend%C3%B8rs/", Type.Outdoors));
                _productsTool.AddRange(Start("https://www.flugger.dk/maling-tapet/filt-v%C3%A6v-og-savsmuldstapet/",
                    Type.Tools));
                _productsOther.AddRange(Start("https://www.flugger.dk/maling-tapet/dekoration/", Type.Others));
                _productsTool.AddRange(Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/spartler-sandpapir/",
                    Type.Tools));
                _productsOther.AddRange(Start("https://www.flugger.dk/maling-tapet/tapetkl%C3%A6ber/", Type.Others));
                _productsTool.AddRange(Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/bakker-spande/", Type.Tools));
                
                
                PopulateProducts(_productsIndoor, _allProductTypes[0]);
                PopulateProducts(_productsOutdoor, _allProductTypes[1]);
                PopulateProducts(_productsTool, _allProductTypes[2]);
                PopulateProducts(_productsOther, _allProductTypes[3]);

                _unitOfWork.ProductsType.AddRange(_allProductTypes);
                _unitOfWork.Website.Add(_website);
                _unitOfWork.Complete();
            }
            catch (Exception e)
            {
                if (e.InnerException != null) 
                    Console.WriteLine(e.InnerException.Message);
                else
                {
                    Console.WriteLine(e.StackTrace);
                }
            }

            Console.WriteLine("Scrap is finished!!!");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        private void PopulateProducts(IList<Product> products, ProductType productType)
        {
            foreach (var product in products)
            {
                product.Website = _website;
                product.ProductType = productType;
                _unitOfWork.Products.Add(product);
            }
        }

        private IList<Product> Start(String urlToScrap, Enum type)
        {
            HtmlDocument htmlDocument =
                ScrappingHelper.GetHtmlDocument(urlToScrap);
            List<HtmlNode> listOfProducts = GetProductsList(htmlDocument);
            return GetScrappedProducts(listOfProducts);
        }

        private List<HtmlNode> GetProductsList(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("product-block")).ToList();
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.Collections.Generic.Dictionary`2[System.String,HtmlAgilityPack.HtmlAttribute]")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: HtmlAgilityPack.HtmlAttribute")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: Entry[System.String,HtmlAgilityPack.HtmlAttribute][]")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: HtmlAgilityPack.HtmlNode[]")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: HtmlAgilityPack.HtmlAttributeCollection")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: Enumerator[HtmlAgilityPack.HtmlNode]")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: Entry[System.Int32,HtmlAgilityPack.HtmlNode][]")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.Collections.Generic.List`1[HtmlAgilityPack.HtmlAttribute]")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: HtmlAgilityPack.HtmlNodeCollection")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.Collections.Generic.List`1[HtmlAgilityPack.HtmlNode]")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: HtmlAgilityPack.HtmlAttribute[]")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.Int32[]")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: HtmlAgilityPack.HtmlTextNode")]
        private IList<Product> GetScrappedProducts(List<HtmlNode> listOfProducts)
        {
            int iteratorForProxies = 0;
            IList<Product> products = new List<Product>();
            Dictionary<String, int> proxyAndPort = ScrappingHelper.GetProxyAndPort();
            IList<String> proxies = proxyAndPort.Keys.ToList();
            IList<int> ports = proxyAndPort.Values.ToList();

            foreach (var product in listOfProducts)
            {
                if (iteratorForProxies > proxyAndPort.Count - 1)
                {
                    iteratorForProxies = 0;
                }

                int amountOfSizesOfAProduct = 0;

                tryAnotherIP:
                Console.WriteLine("Trying ip: " + proxies[iteratorForProxies]);
                try
                {
                    amountOfSizesOfAProduct =
                        GetSize(product, proxies[iteratorForProxies], ports[iteratorForProxies]).Count;


                    Console.WriteLine(amountOfSizesOfAProduct);
                    for (int i = 0; i < amountOfSizesOfAProduct; i++)
                    {
                        if (iteratorForProxies > proxyAndPort.Count - 1)
                        {
                            iteratorForProxies = 0;
                        }

                        Product tempProduct = new Product();
                        tempProduct.Name = GetNameOfProduct(product);
                        tempProduct.Size = GetSize(product, proxies[iteratorForProxies], ports[iteratorForProxies])[i];
                        tempProduct.Price =
                            GetPrice(product, proxies[iteratorForProxies], ports[iteratorForProxies])[i];
                        products.Add(tempProduct);
                        Console.WriteLine(tempProduct.Name);
                        Console.WriteLine("Size:" + tempProduct.Size + " Price: " + tempProduct.Price + " dkk/st");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(proxies[iteratorForProxies] + " failed");
                    if (iteratorForProxies > proxyAndPort.Count - 1)
                    {
                        iteratorForProxies = 0;
                    }
                    else
                    {
                        iteratorForProxies++;
                    }


                    goto tryAnotherIP;
                }
            }

            return products;
        }


        private String GetNameOfProduct(HtmlNode product)
        {
            String tempName;
            tempName = (product.Descendants("h3")
                    .First(node => node.GetAttributeValue("class", "").Equals("product-block-title")).InnerText)
                .Replace("\r\n", "").Trim();
            if (!ScrappingHelper.CheckIfInvalidCharacter(tempName, ScrappingHelper.InvalidCharacter))
            {
                return tempName;
            }

            return ScrappingHelper.FixInvalidCharacter(tempName, ScrappingHelper.InvalidCharacter);
        }


        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: HtmlAgilityPack.HtmlTextNode")]
        private IList<string> GetSize(HtmlNode product, String proxy, int port)
        {
            String url = "https://www.flugger.dk" + GetProductLink(product);

            HtmlDocument htmlDocument =
                ScrappingHelper.GetHtmlDocument(url, proxy, port);

            IList<HtmlNode> listOfSizes = GetProductsWithPriceAndSize(htmlDocument);
            IList<String> sizes = new List<string>();
            String size = "", previousSize = "";
            foreach (HtmlNode category in listOfSizes)
            {
                size = category.SelectSingleNode("div").InnerText.Replace(",", ".");
                if (ScrappingHelper.CheckIfInvalidCharacter(size, ScrappingHelper.InvalidCharacter))
                {
                    size = ScrappingHelper.FixInvalidCharacter(size, ScrappingHelper.InvalidCharacter);
                }

                if (size.Equals(""))
                {

                    return new List<String>(){"No data"};
                }

                if (size.Equals(previousSize))
                {
                    continue;
                }

                sizes.Add(size);
                previousSize = size;
            }

            return sizes;
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.String")]
        private IList<float> GetPrice(HtmlNode product, String proxy, int port)
        {
            String url = "https://www.flugger.dk" + GetProductLink(product);
            HtmlDocument htmlDocument =
                ScrappingHelper.GetHtmlDocument(url, proxy, port);
            IList<HtmlNode> listOfSizes = GetProductsWithPriceAndSize(htmlDocument);
            IList<float> prices = new List<float>();
            String priceAsString = "";
            foreach (HtmlNode category in listOfSizes)
            {
                priceAsString = category.SelectSingleNode("div/span").InnerText;
                if (ScrappingHelper.CheckIfInvalidCharacter(priceAsString, ScrappingHelper.InvalidCharacter))
                {
                    priceAsString =
                        ScrappingHelper.FixInvalidCharacter(priceAsString, ScrappingHelper.InvalidCharacter);
                }

                Match redundantPart = new Regex(@"\,.*").Match(priceAsString);
                if (redundantPart.Value.Length == 0)
                {
                    prices.Add(0);
                }
                else
                {
                    priceAsString = priceAsString.Replace(redundantPart.Value, "").Replace(".", "");
                    prices.Add(float.Parse(priceAsString));
                }
            }

            return prices;
        }


        private List<HtmlNode> GetProductsWithPriceAndSize(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("product-variants-row")).ToList();
        }

        private String GetProductLink(HtmlNode product)
        {
            var link = product.Descendants("a")
                .First(node => node.GetAttributeValue("class", "").Equals("product-blocklinkwrap"));
            var href = link.Attributes["href"].Value;
            if (!ScrappingHelper.CheckIfInvalidCharacter(href, ScrappingHelper.InvalidCharacter))
            {
                return href;
            }

            var correctHref = ScrappingHelper.FixInvalidCharacter(href, ScrappingHelper.InvalidCharacter);
            return correctHref;
        }
    }
}