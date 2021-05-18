using InfoTrack.Business.Dtos;
using InfoTrack.Business.Interfaces;
using InfoTrack.Repository.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace InfoTrack.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[controller]/[action]")]
    public class WebScrapController : Controller
    {
        private readonly IWebScraper _webScraper;
        private readonly ILogger<WebScrapController> _logger;
        public WebScrapController(IWebScraper webScraper, ILogger<WebScrapController> logger)
        {
            _webScraper = webScraper;
            _logger = logger;
        }

        [HttpGet()]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public async Task<IActionResult> Search(string url, string keyword, string toSearch, int? pageSize)
        {
            try
            {
                var result = await _webScraper.DoSearch(url, keyword, toSearch, pageSize);
                return Json(result);
            }
            catch(Exception ex)
            {
                var error = "An error occured while web scraping, Error " + ex.Message;
                _logger.LogError(ex, error);
                throw;
            }
        }
    }
}
