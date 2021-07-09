using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Scraping.ScrappingFluggerDk.Enums;

namespace WebScrapper.Scraping.ScrappingFluggernaerumDk
{
    public class FluggerNaerumDkScrapper
    {
        private DBContext _dbContext;
        private IWebDriver _driver, _driverItem;

        public FluggerNaerumDkScrapper(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void StartScrapping()
        {
            Start("https://flugger-naerum.dk/k/indendoers-maling/", TypesOfProduct.Indoors);
            Start("https://flugger-naerum.dk/k/udendoers-maling/", TypesOfProduct.Outdoors);
            Start("https://flugger-naerum.dk/k/tilbehoer/", TypesOfProduct.Others);
        }

        private void Start(String urlToScrap, Enum type)
        {
            List<String> linksToAllThePages = new List<string>();
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl(urlToScrap);
            _driver.FindElement(By.ClassName("page-numbers"));

        }
    }
}