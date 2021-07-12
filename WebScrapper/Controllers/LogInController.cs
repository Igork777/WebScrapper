using System;
using System.Threading.Tasks;
using WebScrapper.JWT;
using WebScrapper.Scraping.DTO;
using WebScrapper.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebScrapper.Controllers
{
    [Authorize]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private readonly ILogInService _logInService;
        private readonly IJwtAuthenticationManager JwtAuthenticationManager;
        
        public LogInController(ILogInService logInService, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            JwtAuthenticationManager = jwtAuthenticationManager;
            _logInService = logInService;
        }
        
        

        [AllowAnonymous]
        [HttpPost]
        [Route("api/Authenticate")]
        public ActionResult  Authenticate([FromBody] User user)
        {
          var t=  JwtAuthenticationManager.Authenticate(user.userName, user.password);
          if(t == null)
              return Unauthorized();
          
          return Ok(new Token(){token = t});
        }
    }
}