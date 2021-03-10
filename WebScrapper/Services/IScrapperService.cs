using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebScrapper.Services
{
    public interface IScrapperService
    { 
        public Dictionary<string, string> GetAllSuggestions(String name);
    }
}