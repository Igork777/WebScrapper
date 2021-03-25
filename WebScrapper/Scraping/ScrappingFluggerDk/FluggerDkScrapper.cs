using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Scraping.ScrappingFluggerDk.Enums;

namespace WebScrapper.Scraping.ScrappingFluggerDk
{
    public class FluggerDkScrapper
    {
        private DBContext _dbContext;
        private KeyValuePair<String, String> proxyPort;


        public FluggerDkScrapper(DBContext dbContext)
        {
            _dbContext = dbContext;
            proxyPort = ScrappingHelper.getFreshIPAndPort();
        }

        public void StartScrapping()
        {
            Console.WriteLine("Starting new scrap");

            try
            {
                Start("https://www.flugger.dk/maling-tapet/indend%C3%B8rs/", TypesOfProduct.Indoors);
                Start(
                    "https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/tapetv%C3%A6rkt%C3%B8j-kl%C3%A6ber/",
                    TypesOfProduct.Tools);
                Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/pensler-ruller/",
                    TypesOfProduct.Tools);
              
                Start("https://www.flugger.dk/maling-tapet/udend%C3%B8rs/", TypesOfProduct.Outdoors);
                Start("https://www.flugger.dk/maling-tapet/filt-v%C3%A6v-og-savsmuldstapet/",
                    TypesOfProduct.Tools);
                Start("https://www.flugger.dk/maling-tapet/dekoration/", TypesOfProduct.Others);
                Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/spartler-sandpapir/",
                    TypesOfProduct.Tools);
                Start("https://www.flugger.dk/maling-tapet/tapetkl%C3%A6ber/", TypesOfProduct.Others);
                Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/bakker-spande/", TypesOfProduct.Tools);
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

        private void Start(String urlToScrap, Enum type)
        {
            HtmlDocument htmlDocument = null;
            Console.WriteLine("Trying: " + proxyPort.Key);
            tryAnotherIP:
            try
            {
                
                htmlDocument =
                    ScrappingHelper.GetHtmlDocument(urlToScrap, proxyPort.Key, Convert.ToInt32(proxyPort.Value));
            }
            catch (Exception e)
            {
                Console.WriteLine(proxyPort.Key + " failed");
                proxyPort = ScrappingHelper.getFreshIPAndPort();
                goto tryAnotherIP;
            }
        

            List<HtmlNode> listOfProducts = GetProductsList(htmlDocument);
            GetScrappedProducts(listOfProducts, type);
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
        private void GetScrappedProducts(List<HtmlNode> listOfProducts, Enum type)
        {
            
           
            foreach (var product in listOfProducts)
            {
                
                tryAnotherIP:
                try
                {
                    Console.WriteLine("Trying ip: " + proxyPort.Key);
                    List<String> proxySize = GetSize(product, proxyPort.Key,
                        Convert.ToInt32(proxyPort.Value));
                     
                    
                    for (int i = 0; i < proxySize.Count; i++)
                    {
                        Product tempProduct = new Product();
                        tempProduct.Name = ScrappingHelper.RemoveDiacritics(GetNameOfProduct(product).Trim());
                        tempProduct.Size = proxySize[i];
                        tempProduct.Price =
                            GetPrice(product, proxyPort.Key,
                                Convert.ToInt32(proxyPort.Value))[i];
                        tempProduct.PathToImage = "No data";
                        tempProduct.ProductTypeId = Convert.ToInt32(type);
                        tempProduct.WebsiteId = 1;
                        ScrappingHelper.SaveOrUpdate(_dbContext, tempProduct);
                        Console.WriteLine(tempProduct.Name);
                        Console.WriteLine("Size:" + tempProduct.Size + " Price: " + tempProduct.Price + " dkk/st");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(proxyPort.Key + " failed");
                    proxyPort = ScrappingHelper.getFreshIPAndPort();
                    goto tryAnotherIP;
                }
            }
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
        private List<string> GetSize(HtmlNode product, String proxy, int port)
        {
            String url = "https://www.flugger.dk" + GetProductLink(product);

           
         
                tryAnotherIP:
                Console.WriteLine("Trying IP: " + proxyPort.Key);
                HtmlDocument htmlDocument = null;
                try
                { 
                    htmlDocument =
                        ScrappingHelper.GetHtmlDocument(url, proxyPort.Key, Convert.ToInt32(proxyPort.Value));
                }
                catch (Exception e)
                {
                
                    Console.WriteLine(proxyPort.Key + " failed");
                    proxyPort = ScrappingHelper.getFreshIPAndPort();
                    goto tryAnotherIP;
                }
               

            IList<HtmlNode> listOfSizes = GetProductsWithPriceAndSize(htmlDocument);
            List<String> sizes = new List<string>();
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
                    return new List<String>() {"No data"};
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
            tryAnotherIP:
            HtmlDocument htmlDocument = null;
            try
            { 
                htmlDocument =
                    ScrappingHelper.GetHtmlDocument(url, proxyPort.Key, Convert.ToInt32(proxyPort.Value));
            }
            catch (Exception e)
            {
                
                Console.WriteLine(proxyPort.Key + " failed");
                proxyPort = ScrappingHelper.getFreshIPAndPort();
                goto tryAnotherIP;
            }
            
            
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