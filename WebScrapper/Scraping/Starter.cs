using System;
using System.Linq;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.FluggerHorsensDk;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;

namespace WebScrapper.Scraping
{
    public class Starter
    {
        private FluggerHorsensDkScrapper _fluggerHorsensDkScrapper;
        private FluggerHelsingorDkScrapper _fluggerHelsingorDkScrapper;
        private FluggerDkScrapper _fluggerDk;
        private Maling_halvprisDk _malingHalvprisDk;
        private DBContext _dbContext;

        public Starter()
        {
            _dbContext = new DBContext();

            _fluggerDk = new FluggerDkScrapper(_dbContext);
            _fluggerHorsensDkScrapper = new FluggerHorsensDkScrapper(_dbContext);
            _fluggerHelsingorDkScrapper = new FluggerHelsingorDkScrapper(_dbContext);
            _malingHalvprisDk = new Maling_halvprisDk(_dbContext);
        }

        public void Start()
        {
            DBContext dbContext = new DBContext();
            int amountOfProductType = dbContext.ProductType.Count();
            if (amountOfProductType == 0)
            {
                dbContext.ProductType.AddRange(ScrappingHelper._allProductTypes);
                dbContext.SaveChanges();
            }

            int amountOfWebsites = dbContext.Website.Count();
            if (amountOfWebsites == 0)
            {
                dbContext.Website.AddRange(ScrappingHelper._allWebsites);
                dbContext.SaveChanges();
            }

            int amountOfUsers = dbContext.Users.Count();
            if (amountOfUsers == 0)
            {
                dbContext.Users.Add(new User() {userName = "Cliff", password = ScrappingHelper.hashData("CliffChecksEverybody")});
                dbContext.SaveChanges();
            }

         
            // _fluggerDk.StartScrapping();
           //  _fluggerHelsingorDkScrapper.StartScrapping();
            // _fluggerHorsensDkScrapper.StartScrapping();
            _malingHalvprisDk.StartScrapping();
        }
    }
}