using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrack.Repository.Queries
{
    public abstract class BaseQuery
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 100;
    }
}
