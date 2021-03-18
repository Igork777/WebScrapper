using System;
using System.Collections.Generic;
using System.Linq;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.Helpers;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;

namespace WebScrapper.Services
{
    public class LogInService :ILogInService
    {
        private DBContext _dbContext;
        public LogInService(DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public bool CheckIfNameAndPasswordsCorrespond(User user)
        {
            String hashedPassword = ScrappingHelper.hashData(user.password);
            String username = user.userName;
            List<User> CorrespondingUsers = _dbContext.Users.Where(user => user.userName.Equals(username)).ToList()
                .Where(user => user.password.Equals(hashedPassword)).ToList(); 
            if (CorrespondingUsers.Count > 0)
                return true;
            return false;
        }
    }
}