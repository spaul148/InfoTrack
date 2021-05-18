using InfoTrack.Repository.Entities;
using InfoTrack.Repository.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrack.Repository.Repositories.Contracts
{
    public interface IUrlSearchRepository
    {
        Task<UrlSearchResult> GetSearchResult(UrlSearchQuery urlSearchQuery);
    }
}
