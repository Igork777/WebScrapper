using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebScrapper.Services;

namespace WebScrapper.Controllers
{
    [ApiController]
    public class ScrapperController
    {
        private IScrapperService _scrapperService;

        public ScrapperController(IScrapperService scrapperService)
        {
            _scrapperService = scrapperService;
        }

        [HttpGet]
        [Route("api/suggestions/{name}")]
        public Dictionary<string, string> GetAllSugestion(string name)
        {
            return _scrapperService.GetAllSuggestions(name);
        }
        
    }
}