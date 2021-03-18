using Microsoft.AspNetCore.Mvc;
using WebScrapper.Scraping.DTO;
using WebScrapper.Services;

namespace WebScrapper.Controllers
{
    [ApiController]
    public class LogInController
    {
        private ILogInService _logInService;

        public LogInController(ILogInService logInService)
        {
            _logInService = logInService;
        }

        [HttpPost]
        [Route("api/login")]
        public bool login([FromBody] User user)
        {
           return _logInService.CheckIfNameAndPasswordsCorrespond(user);
        }
    }
}