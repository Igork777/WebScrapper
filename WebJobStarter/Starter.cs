using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebJobStarter.DbContext;
using WebJobStarter.DTO;
using WebJobStarter.FluggerHorsensDk;
using WebJobStarter.Helpers;
using WebJobStarter.ScrapperFluggerDk;
using WebJobStarter.ScrappingFluggernaerumDk;
using WebJobStarter.ScrappingMaligHalvprisDk;

namespace WebJobStarter
{
    public class Starter
    {
        private FluggerHorsensDkScrapper _fluggerHorsensDkScrapper;
        private Flugger_HelsingorDkScrapper _fluggerHelsingorDkScrapper;
        private FluggerDkScrapper _fluggerDk;
        private Maling_halvprisDkScrapper _malingHalvprisDk;
        private FluggerNaerumDkScrapper fluggerNaerum;
        private DBContext _dbContext;

        public Starter(DBContext dbContext)
        {
            _dbContext = dbContext;
            _fluggerDk = new FluggerDkScrapper(_dbContext);
            _fluggerHorsensDkScrapper = new FluggerHorsensDkScrapper(_dbContext);
            _fluggerHelsingorDkScrapper = new Flugger_HelsingorDkScrapper(_dbContext);
            _malingHalvprisDk = new Maling_halvprisDkScrapper(_dbContext);
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
            _malingHalvprisDk.StartScrapping();
            _fluggerDk.StartScrapping();
            _fluggerHelsingorDkScrapper.StartScrapping();
            fluggerNaerum.StartScrapping();
            _fluggerHorsensDkScrapper.StartScrapping();
            ScrappingHelper.removeDeletedProductsFromDB(_dbContext);
        }
    }
}