using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Web;
using WebScrapper.Scraping.ScrappingFluggerDk;

namespace WebScrapper.Scraping
{
    public class ScrapperFluggerDk
    {
        private ProductsScrapper _productsScrapper;

        public ScrapperFluggerDk()
        {
            _productsScrapper = new ProductsScrapper();
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: HtmlAgilityPack.HtmlNode")]
        public void StartScrapping()
        {
            Console.WriteLine("Starting new scrap");
            _productsScrapper.Start("https://www.flugger.dk/maling-tapet/indend%C3%B8rs/");
            _productsScrapper.Start("https://www.flugger.dk/maling-tapet/udend%C3%B8rs/");
            _productsScrapper.Start("https://www.flugger.dk/maling-tapet/dekoration/");
            _productsScrapper.Start("https://www.flugger.dk/maling-tapet/filt-v%C3%A6v-og-savsmuldstapet/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/spartler-sandpapir/");
            _productsScrapper.Start("https://www.flugger.dk/maling-tapet/tapetkl%C3%A6ber/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/pensler-ruller/");
            _productsScrapper.Start(
                "https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/bakker-spande/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/spartel-fuge-kit/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/spartler-sandpapir/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/afd%C3%A6kning/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/reng%C3%B8ring/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/tapetv%C3%A6rkt%C3%B8j-kl%C3%A6ber/");
            Console.WriteLine("Scrap is finished!!!");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}