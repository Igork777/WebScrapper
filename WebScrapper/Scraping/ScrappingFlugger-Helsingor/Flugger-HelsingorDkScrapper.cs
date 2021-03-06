using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
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

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: System.String")]
        public void StartScrapping()
        {
            Console.WriteLine("Starting new scrap");
            try
            {
               
                _unitOfWork.ProductsType.AddRange(ScrappingHelper._allProductTypes);
                _unitOfWork.Website.Add(ScrappingHelper._allWebsites[1]);
                _productsIndoor.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/indendoers-maling/loft-og-vaeg/",
                    TypesOfProduct.Indoors));
                PopulateProducts(_productsIndoor, ScrappingHelper._allProductTypes[0]);
                _unitOfWork.Complete();
                
                _productsIndoor.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/indendoers-maling/trae-og-metal/",
                    TypesOfProduct.Indoors));
                _productsIndoor.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/indendoers-maling/grundere-indendors-maling/",
                    TypesOfProduct.Indoors));
          
                _productsOutdoor.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/udendors-maling/facade/", TypesOfProduct.Outdoors));
                _productsOutdoor.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/udendors-maling/traebeskyttelse-og-metalmaling/",
                    TypesOfProduct.Outdoors));
                _productsOutdoor.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/udendors-maling/grundere-udendors-maling/",
                    TypesOfProduct.Outdoors));
          
                _productsOther.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvlak-gulvbehandling/",
                    TypesOfProduct.Others));
                _productsOther.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvmaling/", TypesOfProduct.Others));
              _productsOther.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/gulvbehandling/gulvolie-gulvbehandling/",
                    TypesOfProduct.Others));
           _productsOther.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/moebelpleje/", TypesOfProduct.Others));
             _productsOther.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/linolier/", TypesOfProduct.Others));
           _productsOther.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/handelsvarer/bejdse/", TypesOfProduct.Others));
          
           _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/tilbehor/rengoring-og-afdaekning/",
                    TypesOfProduct.Tools));
             _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/tilbehor/spartel-og-fuge/", TypesOfProduct.Tools));
           _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/tilbehor/vaegbeklaedning-og-klaebere/",
                    TypesOfProduct.Tools));
            _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/bakker-og-spande/", TypesOfProduct.Tools));
          _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/malerruller/", TypesOfProduct.Tools));
          _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/pensler/", TypesOfProduct.Tools));
           _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/spartler-og-skrabere/",
                    TypesOfProduct.Others));
           _productsTool.AddRange(Start("https://flugger-helsingor.dk/vare-kategori/vaerktoj/vaegbeklaedning/", TypesOfProduct.Tools));
           
          
            
          
           PopulateProducts(_productsOutdoor, ScrappingHelper._allProductTypes[1]);
           PopulateProducts(_productsTool, ScrappingHelper._allProductTypes[2]);
           PopulateProducts(_productsOther, ScrappingHelper._allProductTypes[3]);
           
         
           
           
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
            HtmlDocument initialHtmlDocument =
                ScrappingHelper.GetHtmlDocument(urlToScrap);
            List<HtmlDocument> documentsByPage = new List<HtmlDocument>() {initialHtmlDocument};
            IList<String> links = GetAllPagesLinks(initialHtmlDocument);
            ;
            foreach (String link in links)
            {
                documentsByPage.Add(ScrappingHelper.GetHtmlDocument(link));
            }

            return GetScrappedProducts(documentsByPage);
        }

        private List<HtmlNode> GetProductsList(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("wcbd_product_details")).ToList();
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.String")]
        private IList<Product> GetScrappedProducts(List<HtmlDocument> listOfDocuments)
        {
            int iteratorForProxies = 0;
            Dictionary<String, int> proxyAndPort = ScrappingHelper.GetProxyAndPort();
            IList<String> proxies = proxyAndPort.Keys.ToList();
            IList<int> ports = proxyAndPort.Values.ToList();
            List<HtmlNode> productsList = new List<HtmlNode>();
            List<String> hrefs = new List<String>();
            List<Product> products = new List<Product>();
            foreach (HtmlDocument htmlDocument in listOfDocuments)
            {
                productsList.AddRange(GetProductsList(htmlDocument));
                hrefs.AddRange(GetProductsLink(htmlDocument));
            }

            if (productsList.Count != hrefs.Count)
                throw new Exception("Amount of elements in product list and hrefs are not the same");

            int i = 0;
            tryAnotherIP:
            Console.WriteLine("Trying ip: " + proxies[iteratorForProxies]);
            try
            {
                for (; i < productsList.Count; i++)
                {
                    int sizes = GetSizes(hrefs[i]).Count;
                    int prices = GetPrices(hrefs[i], sizes).Count;
                    // GetSizes(hrefs[i], proxies[iteratorForProxies], ports[iteratorForProxies]);
                    List<String> allSizes = GetSizes(hrefs[i]);
                    List<String> allPrices = GetPrices(hrefs[i], sizes).ToList();
                    for (int j = 0; j < prices; j++)
                    {
                        Product tempProduct = new Product();
                        tempProduct.Name = GetNameOfProduct(productsList[i]);
                        Console.WriteLine("Name: " + tempProduct.Name);
                        try
                        {
                            tempProduct.Size = allSizes[j];
                        }
                        catch (Exception ex)
                        {
                            tempProduct.Size = "No data";
                        }

                        Console.WriteLine("Size: " + tempProduct.Size);
                        tempProduct.Price = allPrices[j];
                        Console.WriteLine("Price: " + tempProduct.Price);
                        Console.WriteLine("------------------------------------------");
                        products.Add(tempProduct);
                    }

                    // GetSizes(hrefs[i], proxies[iteratorForProxies], ports[iteratorForProxies]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(proxies[iteratorForProxies] + " failed");
                if (iteratorForProxies >= proxyAndPort.Count - 1)
                {
                    iteratorForProxies = 0;
                }
                else
                {
                    iteratorForProxies++;
                }

                goto tryAnotherIP;
            }

            return products;
        }

        // private IList<string> GetPrices(String url, String proxy, int port)
        private HashSet<string> GetPrices(string url, int amountOfSizes)
        {
            int counter = 0;
            Regex correctRowRegex = new Regex("var aepc_wc_add_to_cart =.*");
            Regex JSONobjRegex = new Regex("{.*};");
            Regex idsRegex = new Regex("\"[0-9]+\":");
            Regex valueRegex = new Regex("\"value\":[0-9]+");
            // HtmlDocument htmlDocument =
            //     ScrappingHelper.GetHtmlDocument(url, proxy, port);
            HtmlDocument htmlDocument =
                ScrappingHelper.GetHtmlDocument(url);
            HashSet<string> correstedValues = null;
            try
            {
                var pricesAsJson = htmlDocument.DocumentNode.Descendants("script").First(node =>
                    node.GetAttributeValue("id", "").Equals("aepc-pixel-events-js-extra"));
                string correctRowMatched = correctRowRegex.Match(pricesAsJson.InnerText).Value;
                string preparedJSON = JSONobjRegex.Match(correctRowMatched).Value;
                preparedJSON = preparedJSON.Replace(";", "");
                StringBuilder JSON = new StringBuilder(preparedJSON);
                MatchCollection ids = idsRegex.Matches(JSON.ToString());
                MatchCollection values = valueRegex.Matches(JSON.ToString());

                correstedValues = new HashSet<string>();
                foreach (Match val in values)
                {
                    if (counter > amountOfSizes)
                    {
                        break;
                    }

                    correstedValues.Add(val.Value.Replace("\"value\":", ""));
                    counter++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured: " + e);
            }


            return correstedValues;
        }

        // private IList<string> GetSizes(String url, String proxy, int port)
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.String")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.String")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
            MessageId = "type: System.String")]
        private List<string> GetSizes(String url)
        {
            HtmlDocument htmlDocument =
                ScrappingHelper.GetHtmlDocument(url);

            List<String> sizes = new List<string>();
            Regex checkOption = new Regex("[0-9].*");
            var select = htmlDocument.DocumentNode.Descendants("select")
                .FirstOrDefault(node => node.GetAttributeValue("id", "").Equals("pa_stoerrelse_ml"));
            if (select == null)
            {
                select = htmlDocument.DocumentNode.Descendants("select").FirstOrDefault(node =>
                    node.GetAttributeValue("id", "").Equals("pa_stoerrelse_mm"));
                if (select == null)
                {
                    return new List<string>();
                }
            }

            foreach (var i in select.ChildNodes)
            {
                if (!checkOption.IsMatch(i.InnerText))
                {
                    continue;
                }

                String size = i.InnerText.Replace(",", ".");
                if (ScrappingHelper.CheckIfInvalidCharacter(i.InnerText, ScrappingHelper.InvalidCharacter))
                {
                    var fixedText = ScrappingHelper.FixInvalidCharacter(size, ScrappingHelper.InvalidCharacter);
                    sizes.Add(fixedText);
                }
                else
                {
                    sizes.Add(size);
                }
            }

            return sizes;
        }


        private List<String> GetProductsLink(HtmlDocument product)
        {
            IEnumerable<HtmlNode> allLinks = product.DocumentNode.Descendants("a").Where(node =>
                node.GetAttributeValue("class", "")
                    .Equals("woocommerce-LoopProduct-link woocommerce-loop-product__link")).ToList();
            List<String> hrefs = new List<string>();
            foreach (var link in allLinks)
            {
                hrefs.Add(link.Attributes["href"].Value);
            }

            return hrefs;
        }

        private String GetNameOfProduct(HtmlNode product)
        {
            String tempName;
            tempName = product.Descendants("h2").First().InnerText.Replace("\r\n", "").Trim();
            if (!ScrappingHelper.CheckIfInvalidCharacter(tempName, ScrappingHelper.InvalidCharacter))
            {
                return tempName;
            }

            return ScrappingHelper.FixInvalidCharacter(tempName, ScrappingHelper.InvalidCharacter);
        }

        private IList<String> GetAllPagesLinks(HtmlDocument htmlDocument)
        {
            IList<String> links = new List<string>();
            IList<HtmlNode> nodes = htmlDocument.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("class", "").Equals("page-numbers")).ToList();
            foreach (var a in nodes)
            {
                links.Add(a.Attributes["href"].Value);
            }

            return links;
        }
    }
}