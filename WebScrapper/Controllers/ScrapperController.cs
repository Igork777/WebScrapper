using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebScrapper.Scraping.DTO;
using WebScrapper.Services;

namespace WebScrapper.Controllers
{
    [ApiController]
    [Authorize]
    public class ScrapperController : ControllerBase
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

        [HttpGet]
        [Route("api/products/{name}")]
        public Dictionary<String, IList<Product>> GetAllProduct(String name)
        {
            return _scrapperService.GetAllProducts(name);
        }

        [HttpGet]
        [Route("api/products/lowerPrice")]
        public IEnumerable<ComparedProduct> getComparedProduct()
        {
            return _scrapperService.GetAllProductsThatAreWorseThenFluggers();
        }

        [HttpGet]
        [Route("api/products/latestPriceChange")]
        public IEnumerable<Product> getLatestPriceChange()
        {
            return _scrapperService.GetLatestPriceUpdatedProduct();
        }



    }
}