using InfoTrack.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrack.Business.Interfaces
{
    public interface IWebScraper
    {
        Task<IEnumerable<UrlParseResult>> DoSearch(string url, string keyword, string toSearch, int? pageSize);
    }
}
