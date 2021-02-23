using System;

namespace WebScrapper.Scraping
{
    public class ScrapperFluggerDk
    {
        private IndoorsProductsScrapper _indoorsProductsScrapper;

        public ScrapperFluggerDk()
        {
            _indoorsProductsScrapper = new IndoorsProductsScrapper();
        }

        public void StartScrapping()
        {
            _indoorsProductsScrapper.Start("https://www.flugger.dk/maling-tapet/filt-v%C3%A6v-og-savsmuldstapet/");
            
        }
    }
}