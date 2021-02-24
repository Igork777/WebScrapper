using System;
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

        public void StartScrapping()
        {
            _productsScrapper.Start("https://www.flugger.dk/maling-tapet/udend%C3%B8rs/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/pensler-ruller/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/bakker-spande/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/spartel-fuge-kit/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/spartler-sandpapir/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/afd%C3%A6kning/");
            _productsScrapper.Start("https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/afd%C3%A6kning/");
            _productsScrapper.Start(
                "https://www.flugger.dk/malerv%C3%A6rkt%C3%B8j/tapetv%C3%A6rkt%C3%B8j-kl%C3%A6ber/");
        }
    }
}