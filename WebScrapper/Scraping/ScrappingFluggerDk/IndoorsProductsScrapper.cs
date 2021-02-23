using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;

namespace WebScrapper.Scraping
{
    public class IndoorsProductsScrapper
    {
        private Regex invalidCharacter = new Regex(@"&#[0-9]+[;]|&[A-Za-z]+[;]");

        public void Start(String urlToScrap)
        {
            HtmlDocument htmlDocument =
                ScrappingHelper.GetHtmlDocument(urlToScrap);

            List<HtmlNode> listOfProducts = GetProductsList(htmlDocument);
            GetScrappedProducts(listOfProducts);
        }

        private List<HtmlNode> GetProductsList(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("product-block")).ToList();
        }

        private IList<Product> GetScrappedProducts(List<HtmlNode> listOfProducts)
        {
            IList<Product> products = new List<Product>();
            Product tempProduct = new Product();
            foreach (var product in listOfProducts)
            {
                tempProduct.Name = GetNameOfProduct(product);
                tempProduct.SizeToPrice = GetSizeAndCorrespondingPrice(product);
                products.Add(tempProduct);
                Console.WriteLine(tempProduct.Name);
                foreach (KeyValuePair<float, int> sizeAndPrice in tempProduct.SizeToPrice)
                {
                    Console.WriteLine("Size: " + sizeAndPrice.Key + "L Price: "+ sizeAndPrice.Value + " dkk/st");
                }
            }

            return products;
        }


        private String GetNameOfProduct(HtmlNode product)
        {
            String tempName;
            IList<String> names = new List<String>();
            tempName = (product.Descendants("h3")
                .First(node => node.GetAttributeValue("class", "").Equals("product-block-title")).InnerText);
            var replacedName = tempName.Replace("\r\n", "").Trim();
            if (!ScrappingHelper.CheckIfInvalidCharacter(replacedName, invalidCharacter))
            {
                return replacedName;
            }

            return FixInvalidCharacter(replacedName, invalidCharacter);
        }
        
  


        private String FixInvalidCharacter(String name, Regex invalidCharacter)
        {
            String fixedName = name;
            MatchCollection incorrectCharacters = invalidCharacter.Matches(name);
            foreach (Match incorrectCharacter in incorrectCharacters)
            {
                fixedName = fixedName.Replace(incorrectCharacter.Value,
                    HttpUtility.HtmlDecode(incorrectCharacter.Value));
            }

            return fixedName;
        }

        private Dictionary<float, int> GetSizeAndCorrespondingPrice(HtmlNode product)
        {
            HtmlDocument htmlDocument = ScrappingHelper.GetHtmlDocument("https://www.flugger.dk" + getProductLink(product));
            IList<HtmlNode> listOfSizes = GetProductsWithPriceAndSize(htmlDocument);
            String size = "", price = "", previousSize= "";
            Dictionary<float, int> categories = new Dictionary<float, int>();
            foreach (HtmlNode category in listOfSizes)
            {
                size = category.SelectSingleNode("div").InnerText.Replace(" ", "").Replace("L", "").Replace(",", ".");
                price = category.SelectSingleNode("div/span").InnerText;
                if (size.Equals("") || price.Equals(""))
                {
                    return new Dictionary<float, int>
                    {
                        {0.0F, 0}
                    };
                }
                Match redundantPart = new Regex(@"\,.*").Match(price);
                price = price.Replace(redundantPart.Value, "").Replace(".", "");
                

                if (size.Equals(previousSize))
                {
                    continue;
                }
                categories.Add(float.Parse(size), Int32.Parse(price));
                previousSize = size;
            }

            if (categories.Count ==0)
            {
                return new Dictionary<float, int>
                {
                    {0.0F, 0}
                };
            }
            return categories;
        }


        private List<HtmlNode> GetProductsWithPriceAndSize(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("product-variants-row")).ToList();
        }

        private String getProductLink(HtmlNode product)
        {
            var link = product.Descendants("a")
                .First(node => node.GetAttributeValue("class", "").Equals("product-blocklinkwrap"));
            var href = link.Attributes["href"].Value;
            if (!ScrappingHelper.CheckIfInvalidCharacter(href, invalidCharacter))
            {
                return href;
            }

            var correctHref = FixInvalidCharacter(href, invalidCharacter);
            return correctHref;
        }
    }
}