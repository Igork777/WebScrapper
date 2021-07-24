using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.FluggerHorsensDk;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Scraping.ScrappingFluggernaerumDk;

namespace WebScrapper.Scraping
{
    public class Starter
    {
        private FluggerHorsensDkScrapper _fluggerHorsensDkScrapper;
        private FluggerHelsingorDkScrapper _fluggerHelsingorDkScrapper;
        private FluggerDkScrapper _fluggerDk;
        private Maling_halvprisDk _malingHalvprisDk;
        private FluggerNaerumDkScrapper fluggerNaerum;
        private DBContext _dbContext;

        public Starter(DBContext dbContext)
        {
            _dbContext = dbContext;
            _fluggerDk = new FluggerDkScrapper(_dbContext);
            _fluggerHorsensDkScrapper = new FluggerHorsensDkScrapper(_dbContext);
            _fluggerHelsingorDkScrapper = new FluggerHelsingorDkScrapper(_dbContext);
            _malingHalvprisDk = new Maling_halvprisDk(_dbContext);
            fluggerNaerum = new FluggerNaerumDkScrapper(_dbContext);
        }

        public void Start()
        {
            _dbContext.Database.Migrate();
            int amountOfProductType = _dbContext.ProductType.Count();
            if (amountOfProductType == 0)
            {
                _dbContext.ProductType.AddRange(ScrappingHelper._allProductTypes);
                _dbContext.SaveChanges();
            }

            int amountOfWebsites = _dbContext.Website.Count();
            if (amountOfWebsites == 0)
            {
                _dbContext.Website.AddRange(ScrappingHelper._allWebsites);
                _dbContext.SaveChanges();
            }

            int amountOfUsers = _dbContext.Users.Count();
            if (amountOfUsers < 2)
            {
                try
                {
                    _dbContext.Users.Add(new User()
                        {userName = "Cliff", password = ScrappingHelper.hashData("CliffChecksEverybody")});
                }
                catch (Exception e)
                {
                    // ignored
                }

                try
                {
                    _dbContext.Users.Add(new User()
                        {userName = "Mikkel", password = ScrappingHelper.hashData("MikkelChecksEverybody")});
                }
                catch (Exception e)
                {
                    // ignored
                }

                _dbContext.SaveChanges();
            }


            ScrappingHelper.LoadAllProducts(_dbContext);
            _fluggerHorsensDkScrapper.StartScrapping();
            _fluggerDk.StartScrapping();
            fluggerNaerum.StartScrapping();
            _malingHalvprisDk.StartScrapping();
            _fluggerHelsingorDkScrapper.StartScrapping();
            ScrappingHelper.removeDeletedProductsFromDB(_dbContext);
        }
    }
}