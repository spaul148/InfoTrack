using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrack.Business.Dtos
{
    public class UrlParseResult
    {
        public int Index { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public MatchedIn MatchedIn { get; set; }
    }
}
