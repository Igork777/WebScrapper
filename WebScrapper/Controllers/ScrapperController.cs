using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebScrapper.Scraping.DTO;
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
        public IList<Suggestion> GetAllSugestion(string name)
        {
            return _scrapperService.GetAllSuggestions(name);
        }
        
    }
}