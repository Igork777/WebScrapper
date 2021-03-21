using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Scraping.ScrappingFluggerDk.Enums;

namespace WebScrapper.Scraping
{
    public class Maling_halvprisDk
    {
        private DBContext _dbContext;
        int iterator = 0;
        Dictionary<String, String> proxyAndPort = ScrappingHelper.getIPAndPort();
        List<String> proxies = new List<string>();
        List<String> ports = new List<string>();
        
        public Maling_halvprisDk(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void StartScrapping()
        {
            proxies.AddRange(proxyAndPort.Keys);
            ports.AddRange(proxyAndPort.Values);
            Console.WriteLine("Starting new scrap");
            Start("https://www.maling-halvpris.dk/butik-kob-maling/flugger/flugger-inde/", TypesOfProduct.Indoors);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/ral-tex/ral-tex_inde/", TypesOfProduct.Indoors);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/ral-tex/ral-tex_ude/", TypesOfProduct.Outdoors);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/beckers-inde/", TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/panel-og-traemaling/",
                TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/gulvmaling-fra-beckers/",
                TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/glasfilt/", TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/beckers-ude/", TypesOfProduct.Outdoors);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/beckers/facademaling/", TypesOfProduct.Others);

           
            Start("https://www.maling-halvpris.dk/butik-kob-maling/flugger/flugger-ude/", TypesOfProduct.Outdoors);

            Start("https://www.maling-halvpris.dk/butik-kob-maling/dyrup-inde/", TypesOfProduct.Others);


            Start("https://www.maling-halvpris.dk/butik-kob-maling/junckers/", TypesOfProduct.Others);

            Start("https://www.maling-halvpris.dk/butik-kob-maling/gori/afvask-og-algebehandling/",
                TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/gori/gori-professionel/", TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/gori/gori-daekkende/", TypesOfProduct.Others);

            Start("https://www.maling-halvpris.dk/butik-kob-maling/gori/gori-daekkende/", TypesOfProduct.Others);
            Start("https://www.maling-halvpris.dk/butik-kob-maling/terrasseolie/", TypesOfProduct.Others);
        }

        private void Start(String urlToScrap, Enum type)
        {
          //  ScrappingHelper.RenewIpAndPorts();
            HtmlDocument htmlDocument = null;
            tryAnotherIP:
            Console.WriteLine("Trying IP: " + proxies[iterator]);
            try
            {
                htmlDocument =
                    ScrappingHelper.GetHtmlDocument(urlToScrap, proxies[iterator], Convert.ToInt32(ports[iterator]));
            }
            catch (Exception e)
            {
                if (iterator == proxyAndPort.Count - 1)
                {
                    proxyAndPort = ScrappingHelper.getIPAndPort();
                    proxies.AddRange(proxyAndPort.Keys);
                    ports.AddRange(proxyAndPort.Values);
                    iterator = 0;
                }
                else
                {
                    iterator++;
                }
                Console.WriteLine(proxies[iterator] + " failed");
                goto tryAnotherIP;
            }

            GetProductsList(htmlDocument, type);
        }


        private void GetProductsList(HtmlDocument htmlDocument, Enum type)
        {
            Regex sizeRegex = new Regex("([0-9]+,[0-9]+L)|([0-9]+L)|([0-9]+,[0-9]+ L)|([0-9]+ L)");
            IList<HtmlNode> productsHtmlNode = htmlDocument.DocumentNode.Descendants("a")
                .Where(node =>
                    node.GetAttributeValue("class", "")
                        .Equals("woocommerce-LoopProduct-link woocommerce-loop-product__link")).ToList();
            IList<HtmlNode> forms = htmlDocument.DocumentNode.Descendants("form")
                .Where(node => node.GetAttributeValue("class", "").Equals("variations_form cart")).ToList();
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

                    product.Name = ChangeNameToNormal(product.Name);

                    product.PathToImage = productsHtmlNode[i].Descendants("img").First(node =>
                            node.GetAttributeValue("class", "")
                                .Equals("attachment-woocommerce_thumbnail size-woocommerce_thumbnail"))
                        .Attributes["src"]
                        .Value;
                    product.PathToImage =
                        ScrappingHelper.FixInvalidCharacter(product.PathToImage, ScrappingHelper.InvalidCharacter);
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
                                if (IsSecondNumberBiggerThanFirst(firstNumber, lastNumber))
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


                            Product temp = new Product()
                            {
                                Name = product.Name, Price = product.Price, Size = product.Size, WebsiteId = 3,
                                ProductTypeId = Convert.ToInt32(type), PathToImage = product.PathToImage
                            };
                            temp.Name = ScrappingHelper.RemoveDiacritics(temp.Name.Trim());
                            if (temp.Name.Equals(""))
                            {
                                return;
                            }
                            ScrappingHelper.SaveOrUpdate(_dbContext, temp);
                        }
                    }
                }
            }
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

            Console.WriteLine("Before new function: " + rightProductName);
             rightProductName = ReturnNameWhichIsUsedInOtherTables(rightProductName);
             Console.WriteLine("After new function " + rightProductName);

            Console.WriteLine(rightProductName);

            return rightProductName;
        }

        private String ReturnNameWhichIsUsedInOtherTables(String nameToCorrect)
        {
            String temp = nameToCorrect;
          
            List<String> separatedStrings =  nameToCorrect.Split(" ").ToList();
            for (int i = separatedStrings.Count-1; i > 0 ; i--)
            {
                List<Product> products = _dbContext.Product.Where(product => product.WebsiteId != 3).ToList().Where(product => product.Name == nameToCorrect).ToList();
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


        private bool IsSecondNumberBiggerThanFirst(String firstNumber, String lastNumber)
        {
            Regex numberRegex = new Regex("([0-9]+\\.[0-9]+)|([0-9]+)");
            double first = Double.Parse(numberRegex.Match(firstNumber).Value);
            double second = Double.Parse(numberRegex.Match(lastNumber).Value);

            return second < first;
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