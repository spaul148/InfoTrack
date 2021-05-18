using InfoTrack.Business.Dtos;
using InfoTrack.Business.Interfaces;
using InfoTrack.Framework;
using InfoTrack.Repository.Queries;
using InfoTrack.Repository.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Web;

namespace InfoTrack.Business.Implementations
{
    public class GoogleWebScraper : IWebScraper
    {
        private readonly IUrlSearchRepository _urlSearchRepository;
        private readonly IHtmlParser _htmlParser;
        private readonly IThreadingUtility _threadingUtility;
        private const string _mainDivId = "main";
        private const string _divTagName = "div";
        private const string _anchorTagName = "a";
        private const string _hrefAttribute = "href";
        private const string _h3TagName = "h3";
        private readonly string[] _tagsToIgnore = new string[] { "<b" };
        private const string UrlCleanUpToken = "/url?q=";
        public GoogleWebScraper(IUrlSearchRepository urlSearchRepository, IHtmlParser htmlParser, IThreadingUtility threadingUtility)
        {
            _urlSearchRepository = urlSearchRepository;
            _htmlParser = htmlParser;
            _threadingUtility = threadingUtility;
        }

        /// <summary>
        /// This method uses the repository to get data and then searches the results. Right now we only support google 
        /// search engine but adding another one following the interface should be trivial.
        /// 
        /// TODO: THIS METHOD SHOULD BE MADE MULTI THREADED TO ACCOUNT FOR MORE THAN 100 PAGESIZE FOR GOOGLE SEARCH
        /// AND ALSO TO PARALLEL COMPUTE THE MATCHES. THE SEARCH WOULD REQUIRE GETTING NUMBER OF PAGES FROM FIRST CALL 
        /// AND USING THAT TO PAGINATE
        /// </summary>
        /// <param name="url"></param>
        /// <param name="keyword"></param>
        /// <param name="toSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UrlParseResult>> DoSearch(string url, string keyword, string toSearch, int? pageSize = 100)
        {
            if (!url.Contains("www.google.com"))
            {
                throw new NotImplementedException("The search engine only supports google search");
            }

            var urlReferrerPatternToMatch = toSearch.StartsWith("www.") ? toSearch :  $"www.{toSearch}.com";
            var encodedKeyWord = HttpUtility.UrlEncode(keyword);
            var query = new UrlSearchQuery() { Keyword = encodedKeyWord, PageSize = pageSize.Value };
            var htmlResult = await _urlSearchRepository.GetSearchResult(query);
            var html = htmlResult.Html;

            var mainDiv = _htmlParser.ParseTagWithId(html, _divTagName, _mainDivId);
            var h3s = _htmlParser.ParseAllTags(mainDiv, _h3TagName);
            var urlResult = new ConcurrentBag<UrlParseResult>();

            
            //we can do parallel computations here to search
            _threadingUtility.ParallelizeWork(h3s, h3 =>
            {
                var result = new UrlParseResult() { Index = h3.Index };
                MatchedIn? matchedIn = null;
                var anchor = _htmlParser.FindParentTag(h3.Content, html, _anchorTagName, _tagsToIgnore, 1);
                string href = "";

                //search in the url for exact match
                if (!string.IsNullOrWhiteSpace(anchor))
                {
                    href = _htmlParser.GetAttributeValue(anchor, _anchorTagName, _hrefAttribute);
                    href = href?.Replace(UrlCleanUpToken, "");
                    href = href.Substring(0, href.IndexOf("&amp;sa=U&amp;"));
                    if (href?.Contains(urlReferrerPatternToMatch, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        matchedIn = MatchedIn.Url;
                        result.Url = href;
                        result.Content = _htmlParser.RemoveAllHtmlTags(h3.Content);
                    }
                }

                //search in google search results headings
                if (matchedIn == null && h3.Content?.Contains(toSearch, StringComparison.OrdinalIgnoreCase) == true)
                {
                    matchedIn = MatchedIn.Heading;
                    result.Content = _htmlParser.RemoveAllHtmlTags(h3.Content);
                    result.Url = href;
                }

                // search in google search results descriptions
                if (matchedIn == null)
                {
                    var parent = _htmlParser.FindParentTag(h3.Content, html, _divTagName, _tagsToIgnore, 2);
                    if (!string.IsNullOrWhiteSpace(parent))
                    {
                        var description = _htmlParser.FindNextSiblingTag(parent, html, _divTagName, 2);
                        description = _htmlParser.RemoveAllHtmlTags(description);
                        if (description?.Contains(toSearch, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            matchedIn = MatchedIn.Description;
                            result.Content = description;
                            result.Url = href;
                        }
                    }

                }

                if (matchedIn != null)
                {
                    result.MatchedIn = matchedIn.Value;
                    urlResult.Add(result);
                }
            });

            return urlResult.OrderBy(r => r.Index).ToList();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="url"></param>
        /// <param name="keyword"></param>
        /// <param name="toSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UrlParseResult>> DoAsyncSearch(string url, string keyword, string toSearch, int? pageSize = 100)
        {
            int chunkSize = 100;
            int chunks = pageSize > chunkSize ? (int)Math.Ceiling((double)pageSize / chunkSize) : 1;
            var encodedKeyWord = HttpUtility.UrlEncode(keyword);

            var queries = Enumerable.Range(0, chunks).Select(r =>
            {
                var pageIndex = (r * chunkSize) + 1;
                return new UrlSearchQuery() { Keyword = encodedKeyWord, PageSize = chunkSize, PageIndex = pageIndex };
            });

            

            return null;
        }
    }
}
