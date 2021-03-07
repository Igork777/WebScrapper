using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V86.DOM;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Scraping.ScrappingFluggerDk.Enums;
using WebScrapper.Scraping.ScrappingFluggerDk.Repositories;

namespace WebScrapper.Scraping
{
    public class Maling_halvprisDk
    {
        private List<Product> _productsIndoor;
        private List<Product> _productsOutdoor;
        private List<Product> _productsTool;
        private List<Product> _productsOther;
        private UnitOfWork _unitOfWork;

        public Maling_halvprisDk(UnitOfWork unitOfWork)
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
           _productsIndoor.AddRange((Start("https://www.maling-halvpris.dk/butik-kob-maling/ral-tex/ral-tex_inde/", TypesOfProduct.Indoors)));
           _productsOutdoor.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/ral-tex/ral-tex_ude/", TypesOfProduct.Outdoors));
            _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/beckers-inde/", TypesOfProduct.Others));
            _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/panel-og-traemaling/",
                TypesOfProduct.Others));
            _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/gulvmaling-fra-beckers/",
                TypesOfProduct.Others));
            _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/glasfilt/", TypesOfProduct.Others));
            _productsOutdoor.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/beckers-ude/", TypesOfProduct.Outdoors));
            _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/facademaling/", TypesOfProduct.Others));

            _productsIndoor.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/flugger/flugger-inde/", TypesOfProduct.Indoors));
            _productsOutdoor.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/flugger/flugger-ude/", TypesOfProduct.Outdoors));

            _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/dyrup-inde/", TypesOfProduct.Others));
           
           
            _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/junckers/", TypesOfProduct.Others));
           
            _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/gori/afvask-og-algebehandling/",
                TypesOfProduct.Others));
           _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/gori/gori-professionel/", TypesOfProduct.Others));
           _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/gori/gori-daekkende/", TypesOfProduct.Others));
           
            _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/gori/gori-daekkende/", TypesOfProduct.Others));
            _productsOther.AddRange(Start("https://www.maling-halvpris.dk/butik-kob-maling/terrasseolie/", TypesOfProduct.Others));
            
            _unitOfWork.ProductsType.AddRange(ScrappingHelper._allProductTypes);
            _unitOfWork.Website.Add(ScrappingHelper._allWebsites[3]);
            //_unitOfWork.Complete();
            PopulateProducts(_productsIndoor, ScrappingHelper._allProductTypes[0]);
            PopulateProducts(_productsOutdoor, ScrappingHelper._allProductTypes[1]);
            PopulateProducts(_productsTool, ScrappingHelper._allProductTypes[2]);
            PopulateProducts(_productsOther, ScrappingHelper._allProductTypes[3]);
        }

        private IList<Product> Start(String urlToScrap, Enum type)
        {
            ScrappingHelper.RenewIpAndPorts();
            HtmlDocument htmlDocument = null;
            int iterator = 0;
            Console.WriteLine("Trying IP: "+ ScrappingHelper.proxies[iterator]);
            tryAnotherIP:
            try
            {
                // htmlDocument =
                //     ScrappingHelper.GetHtmlDocument(urlToScrap, ScrappingHelper.proxies[iterator], ScrappingHelper.ports[iterator]);
                htmlDocument = ScrappingHelper.GetHtmlDocument(urlToScrap);
            }
            catch (Exception e)
            {
                Console.WriteLine(ScrappingHelper.proxies[iterator] + " failed");
                if (iterator >= ScrappingHelper.proxies.Count - 1)
                {
                    iterator = 0;
                }
                else
                {
                    iterator++;
                }

                goto tryAnotherIP;
            }

            return GetProductsList(htmlDocument);
        }
        
        
        private void PopulateProducts(IList<Product> products, ProductType productType)
        {
            foreach (var product in products)
            {
                product.Website = ScrappingHelper._allWebsites[3];
                product.ProductType = productType;
                _unitOfWork.Products.Add(product);
                Console.WriteLine("Saving Name: " + product.Name + ", Size: " + product.Size+ ", Price: " + product.Price);
                try
                {
                    _unitOfWork.Complete();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    continue;
                }
            }
        }

        private List<Product> GetProductsList(HtmlDocument htmlDocument)
        {
            Regex sizeRegex = new Regex("([0-9]+,[0-9]+L)|([0-9]+L)|([0-9]+,[0-9]+ L)|([0-9]+ L)");
            IList<HtmlNode> productsHtmlNode = htmlDocument.DocumentNode.Descendants("a")
                .Where(node =>
                    node.GetAttributeValue("class", "")
                        .Equals("woocommerce-LoopProduct-link woocommerce-loop-product__link")).ToList();
            IList<HtmlNode> forms = htmlDocument.DocumentNode.Descendants("form")
                .Where(node => node.GetAttributeValue("class", "").Equals("variations_form cart")).ToList();
            List<Product> products = new List<Product>();
            Console.WriteLine();

            
            if (forms.Count != 0)
            {
                for (int i = 0; i < productsHtmlNode.Count; i++)
                {
                    MatchCollection matchCollection = null;
                    IList<String> prices = GetPrices(productsHtmlNode[i]);
                    IList<String> sizes = new List<String>();
                    Product product = new Product();
                    if (forms.Count != 0)
                    {
                        var tempData = forms[i].GetAttributeValue("data-product_variations", "");
                        matchCollection = sizeRegex.Matches(tempData);
                    }

                    product.Name = productsHtmlNode[i].Descendants("h2").First(node =>
                        node.GetAttributeValue("class", "").Equals("woocommerce-loop-product__title")).InnerText;
                    if (ScrappingHelper.CheckIfInvalidCharacter(product.Name, ScrappingHelper.InvalidCharacter))
                    {
                        product.Name =
                            ScrappingHelper.FixInvalidCharacter(product.Name, ScrappingHelper.InvalidCharacter);
                    }

                    product.PathToImage = productsHtmlNode[i].Descendants("img").First(node =>
                        node.GetAttributeValue("class", "")
                            .Equals("attachment-woocommerce_thumbnail size-woocommerce_thumbnail")).Attributes["src"].Value;
                    product.PathToImage = ScrappingHelper.FixInvalidCharacter(product.PathToImage, ScrappingHelper.InvalidCharacter);
                    Console.WriteLine("Path to image: " + product.PathToImage);
                    if (matchCollection != null && matchCollection.Count == 0)
                    {
                        product.Price = "No data";
                        product.Size = "No data";
                    }
                    else
                    {
                        if (matchCollection != null)
                        {
                            String firstNumber = matchCollection[0].Value.Replace(",", ".");
                           
                            if (matchCollection[0].Value.Equals(matchCollection[^1].Value))
                            {
                                sizes.Add(firstNumber);
                            }
                            else
                            {
                                String lastNumber = matchCollection[^1].Value.Replace(",", ".");
                                if(IsSecondNumberBiggerThanFirst(firstNumber, lastNumber))
                                {
                                    sizes.Add(lastNumber);
                                    sizes.Add(firstNumber);
                                }
                                else
                                {
                                    sizes.Add(firstNumber);
                                    sizes.Add(lastNumber);
                                }
                            }
                        }

                        for (int j = 0; j < sizes.Count; j++)
                        {
                            Console.WriteLine("Name: " + product.Name);
                            try
                            {
                                product.Price = prices[j];
                            }
                            catch (Exception e)
                            {
                                product.Price = "No data";
                            }

                            Console.WriteLine("Price: " + product.Price);
                            product.Size = sizes[j];
                            Console.WriteLine("Size: " + product.Size);
                            String concat = "";
                            concat = product.Name + product.Size + product.ProductTypeId + product.WebsiteId +
                                     product.PathToImage;
                            Product temp = new Product()
                                {Name = product.Name, Price = product.Price, Size = product.Size, PathToImage = product.PathToImage,Hash = ScrappingHelper.hashData(concat)};
                            products.Add(temp);
                        }
                    }
                }
            }

            return products;
        }

        private bool IsSecondNumberBiggerThanFirst(String firstNumber, String lastNumber)
        {
          
            Regex numberRegex = new Regex("([0-9]+\\.[0-9]+)|([0-9]+)");
            double first = Double.Parse(numberRegex.Match(firstNumber).Value);
            double second = Double.Parse(numberRegex.Match(lastNumber).Value);
            
            return second<first;
        }

        private IList<String> GetPrices(HtmlNode product)
        {
            Regex priceRegex = new Regex("[1-9]([0-9]{0,2}|\\.)+,");
            IEnumerable<HtmlNode> pricesHtmlNodes = product.Descendants("span")
                .Where(node => node.GetAttributeValue("class", "").Equals("price"));

            IList<String> prices = new List<string>();
            foreach (HtmlNode price in pricesHtmlNodes)
            {
                int iterator = 0;
               

                MatchCollection matchCollection = priceRegex.Matches(price.InnerText);
                foreach (Match match in matchCollection)
                {
                    if (price.Descendants("del").Any() && iterator == 0)
                    {
                        iterator++;
                        continue;
                    }
                    prices.Add(match.Value.Replace(",", ""));
                }
            }

            return prices;
        }
    }
}