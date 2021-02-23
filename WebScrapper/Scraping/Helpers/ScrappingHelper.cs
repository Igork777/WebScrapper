using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebScrapper.Scraping.Helpers
{
    public class ScrappingHelper
    {
        public static HtmlDocument GetHtmlDocument(string url)
        {
            var httpClient = new HttpClient();
            var html = httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html.Result);
            return htmlDocument;
        }
        
        public static bool CheckIfInvalidCharacter(String name, Regex regex)
        {
            return regex.IsMatch(name);
        }
    }
}