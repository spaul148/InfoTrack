using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrack.Repository.Queries
{
    public class UrlSearchQuery : BaseQuery
    {
        public string Url { get; set; }
        public string Keyword { get; set; }
    }
}
