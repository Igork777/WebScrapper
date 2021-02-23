
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
            try
            {
                _indoorsProductsScrapper.Start("https://www.flugger.dk/maling-tapet/indend%C3%B8rs/");
            }
            catch (System.AggregateException e)
            {
                Console.WriteLine("Trying to recollect");
               _indoorsProductsScrapper.Start("https://www.flugger.dk/maling-tapet/indend%C3%B8rs/");
            }
            
        }
    }
}