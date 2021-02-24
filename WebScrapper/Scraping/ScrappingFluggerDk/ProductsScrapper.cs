using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;

namespace WebScrapper.Scraping.ScrappingFluggerDk
{
    public class ProductsScrapper
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
            HashSet<String> names = new HashSet<String>();
            
            foreach (var product in listOfProducts)
            {
                tempProduct.Name = GetNameOfProduct(product);
                tempProduct.SizeToPrice = GetSizeAndCorrespondingPrice(product);
                products.Add(tempProduct);
                Console.WriteLine(tempProduct.Name);
                foreach (KeyValuePair<String, String> sizeAndPrice in tempProduct.SizeToPrice)
                {
                    Console.WriteLine(sizeAndPrice.Key + " Price: "+ sizeAndPrice.Value + " dkk/st");
                }
            }

            return products;
        }


        private String GetNameOfProduct(HtmlNode product)
        {
            String tempName;
            tempName = (product.Descendants("h3")
                .First(node => node.GetAttributeValue("class", "").Equals("product-block-title")).InnerText).Replace("\r\n", "").Trim();
            if (!ScrappingHelper.CheckIfInvalidCharacter(tempName, invalidCharacter))
            {
                return tempName;
            }
            return ScrappingHelper.FixInvalidCharacter(tempName, invalidCharacter);
        }
        
  


        

        private Dictionary<String, String> GetSizeAndCorrespondingPrice(HtmlNode product)
        {
            HtmlDocument htmlDocument = ScrappingHelper.GetHtmlDocument("https://www.flugger.dk" + getProductLink(product));
            IList<HtmlNode> listOfSizes = GetProductsWithPriceAndSize(htmlDocument);
            String size = "", price = "", previousSize= "";
            Dictionary<String, String> categories = new Dictionary<String, String>();
            foreach (HtmlNode category in listOfSizes)
            {
                size = category.SelectSingleNode("div").InnerText.Replace(",", ".");
                price = category.SelectSingleNode("div/span").InnerText;
                if(ScrappingHelper.CheckIfInvalidCharacter(size, invalidCharacter))
                {
                    size = ScrappingHelper.FixInvalidCharacter(size, invalidCharacter);
                }

                if (ScrappingHelper.CheckIfInvalidCharacter(price, invalidCharacter))
                {
                    price = ScrappingHelper.FixInvalidCharacter(price, invalidCharacter);
                }
                if (size.Equals("") || price.Equals(""))
                {
                    return new Dictionary<String, String>
                    {
                        {"0", "0"}
                    };
                }
                Match redundantPart = new Regex(@"\,.*").Match(price);
                price = price.Replace(redundantPart.Value, "").Replace(".", "");
                
                
                if (size.Equals(previousSize))
                {
                    continue;
                }

               
                categories.Add(size, price);
                previousSize = size;
            }

            if (categories.Count ==0)
            {
                return new Dictionary<String, String>
                {
                    {"0", "0"}
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

            var correctHref = ScrappingHelper.FixInvalidCharacter(href, invalidCharacter);
            return correctHref;
        }

      
    }
}