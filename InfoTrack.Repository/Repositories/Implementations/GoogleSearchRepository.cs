using InfoTrack.Repository.Entities;
using InfoTrack.Repository.Queries;
using InfoTrack.Repository.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrack.Repository.Repositories.Implementations
{
    public class GoogleSearchRepository : IUrlSearchRepository
    {
        public async Task<UrlSearchResult> GetSearchResult(UrlSearchQuery urlSearchQuery)
        {
            var query = urlSearchQuery.PageIndex == 0 ?
                $"https://www.google.com/search?num={urlSearchQuery.PageSize}&q={urlSearchQuery.Keyword}" :
                $"https://www.google.com/search?num={urlSearchQuery.PageSize}&q={urlSearchQuery.Keyword}&start={urlSearchQuery.PageIndex}";

            var client = new WebClient();

            var result = await client.DownloadStringTaskAsync(query);
            return new UrlSearchResult() { Html = result };
        }
    }
}
