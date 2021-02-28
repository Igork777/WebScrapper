using System;
using System.Diagnostics.CodeAnalysis;
using Type = WebScrapper.Scraping.ScrappingFluggerDk.Enums.Type;

namespace WebScrapper.Scraping.ScrappingFluggerDk
{
    public class ScrapperFluggerDk
    {
        private ProductsScrapper _productsScrapper;

        public ScrapperFluggerDk()
        {
            _productsScrapper = new ProductsScrapper();
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: HtmlAgilityPack.HtmlNode")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: System.String")]
        public void StartScrapping()
        {
            _productsScrapper.StartScrapping();
        }
    }
}