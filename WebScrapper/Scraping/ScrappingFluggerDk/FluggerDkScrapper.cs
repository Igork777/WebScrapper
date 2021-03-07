using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Scraping.ScrappingFluggerDk.Enums;
using WebScrapper.Scraping.ScrappingFluggerDk.Repositories;

namespace WebScrapper.Scraping.ScrappingFluggerDk
{
    public class FluggerDkScrapper
    {
        private List<Product> _productsIndoor;
        private List<Product> _productsOutdoor;
        private List<Product> _productsTool;
        private List<Product> _productsOther;
        private UnitOfWork _unitOfWork;

        public FluggerDkScrapper(UnitOfWork unitOfWork)
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

            try
            {
                _unitOfWork.Website.Add(ScrappingHelper._allWebsites[0]);
                _unitOfWork.ProductsType.AddRange(ScrappingHelper._allProductTypes);
                _productsTool.AddRange(Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/pensler-ruller/",
                    TypesOfProduct.Tools));
                // _productsIndoor.AddRange(Start("https://www.flugger.dk/maling-tapet/indend%C3%B8rs/", TypesOfProduct.Indoors));
                // _productsTool.AddRange(Start(
                //     "https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/tapetv%C3%A6rkt%C3%B8j-kl%C3%A6ber/", TypesOfProduct.Tools));
                // _productsOutdoor.AddRange(Start("https://www.flugger.dk/maling-tapet/udend%C3%B8rs/", TypesOfProduct.Outdoors));
                // _productsTool.AddRange(Start("https://www.flugger.dk/maling-tapet/filt-v%C3%A6v-og-savsmuldstapet/",
                //     TypesOfProduct.Tools));
                // _productsOther.AddRange(Start("https://www.flugger.dk/maling-tapet/dekoration/", TypesOfProduct.Others));
                // _productsTool.AddRange(Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/spartler-sandpapir/",
                //     TypesOfProduct.Tools));
                // _productsOther.AddRange(Start("https://www.flugger.dk/maling-tapet/tapetkl%C3%A6ber/", TypesOfProduct.Others));
                // _productsTool.AddRange(Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/bakker-spande/", TypesOfProduct.Tools));
                //
                //
                // PopulateProducts(_productsIndoor, ScrappingHelper._allProductTypes[0]);
                // PopulateProducts(_productsOutdoor, ScrappingHelper._allProductTypes[1]);
                PopulateProducts(_productsTool, ScrappingHelper._allProductTypes[2]);
              //  PopulateProducts(_productsOther, ScrappingHelper._allProductTypes[3]);
                
                _unitOfWork.Website.Add(ScrappingHelper._allWebsites[0]);
                _unitOfWork.Complete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
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
                product.Website = ScrappingHelper._allWebsites[0];
                product.ProductType = productType;
                _unitOfWork.Products.Add(product);
            }
        }

        private IList<Product> Start(String urlToScrap, Enum type)
        {
            HtmlDocument htmlDocument = null;
           int iterator = 0;

           htmlDocument =
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
            ScrappingHelper.RenewIpAndPorts();
            int iteratorForProxies = 0;
            IList<Product> products = new List<Product>();

            foreach (var product in listOfProducts)
            {
                if (iteratorForProxies > ScrappingHelper.GetProxyAndPort().Count - 1)
                {
                    iteratorForProxies = 0;
                }

                int amountOfSizesOfAProduct = 0;

                tryAnotherIP:
                Console.WriteLine("Trying ip: " + ScrappingHelper.proxies[iteratorForProxies]);
                try
                {
                    amountOfSizesOfAProduct =
                        GetSize(product, ScrappingHelper.proxies[iteratorForProxies], ScrappingHelper.ports[iteratorForProxies]).Count;

                    String concat = "";
                    for (int i = 0; i < amountOfSizesOfAProduct; i++)
                    {
                        if (iteratorForProxies > ScrappingHelper.proxies.Count - 1)
                        {
                            iteratorForProxies = 0;
                        }

                        Product tempProduct = new Product();
                        tempProduct.Name = GetNameOfProduct(product);
                        tempProduct.Size = GetSize(product, ScrappingHelper.proxies[iteratorForProxies], ScrappingHelper.ports[iteratorForProxies])[i];
                        tempProduct.Price =
                            GetPrice(product, ScrappingHelper.proxies[iteratorForProxies], ScrappingHelper.ports[iteratorForProxies])[i];
                        tempProduct.PathToImage = "No data";
                        concat = tempProduct.Name + tempProduct.Size + tempProduct.ProductType + tempProduct.WebsiteId +
                                 tempProduct.PathToImage;
                        tempProduct.Hash = ScrappingHelper.hashData(concat);
                        Console.WriteLine("Hash: " + tempProduct.Hash);
                        products.Add(tempProduct);
                        Console.WriteLine(tempProduct.Name);
                        Console.WriteLine("Size:" + tempProduct.Size + " Price: " + tempProduct.Price + " dkk/st");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(ScrappingHelper.proxies[iteratorForProxies] + " failed");
                    if (iteratorForProxies > ScrappingHelper.proxies.Count - 1)
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

            // HtmlDocument htmlDocument =
            //     ScrappingHelper.GetHtmlDocument(url, proxy, port);
            HtmlDocument htmlDocument =
                     ScrappingHelper.GetHtmlDocument(url);

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
        private IList<String> GetPrice(HtmlNode product, String proxy, int port)
        {
            String url = "https://www.flugger.dk" + GetProductLink(product);
            // HtmlDocument htmlDocument =
            //     ScrappingHelper.GetHtmlDocument(url, proxy, port);
            HtmlDocument htmlDocument = ScrappingHelper.GetHtmlDocument(url);
            IList<HtmlNode> listOfSizes = GetProductsWithPriceAndSize(htmlDocument);
            IList<String> prices = new List<String>();
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
                    prices.Add("No price");
                }
                else
                {
                    priceAsString = priceAsString.Replace(redundantPart.Value, "").Replace(".", "");
                    prices.Add(priceAsString);
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