using InfoTrack.Business.Implementations;
using InfoTrack.Framework;
using InfoTrack.Repository.Queries;
using InfoTrack.Repository.Repositories.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using InfoTrack.Business.Dtos;
using InfoTrack.Repository.Repositories.Implementations;

namespace InfoTrack.Business.Tests
{
    [TestClass]
    public class GoogleWebScarperTest
    {
        private Mock<IUrlSearchRepository> _urlSearchRepositoryMock;
        private IUrlSearchRepository _urlSearchRepository;
        private IHtmlParser _htmlParser;
        private IThreadingUtility _threadingUtility;

        [TestInitialize]
        public void Init()
        {
            _urlSearchRepositoryMock = new Mock<IUrlSearchRepository>();
            _urlSearchRepository = new GoogleSearchRepository();
            _htmlParser = new GoogleHtmlParser();
            _threadingUtility = new ThreadingUtility();

            var html = File.ReadAllText("Sample.html");
            _urlSearchRepositoryMock.Setup(r => r.GetSearchResult(It.IsAny<UrlSearchQuery>()))
                .Returns(Task.FromResult(new Repository.Entities.UrlSearchResult() { Html = html }));

        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task TestDoSearchWithMockRepository()
        {
            //arrange
            var scraper = new GoogleWebScraper(_urlSearchRepositoryMock.Object, _htmlParser, _threadingUtility);

            //act 
            var result = (await scraper.DoSearch("https://www.google.com", "efiling integration", "infotrack", 100)).ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual(1, result[0].Index);
            Assert.AreEqual(100, result[^1].Index);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task TestDoSearch()
        {
            //arrange
            var scraper = new GoogleWebScraper(_urlSearchRepository, _htmlParser, _threadingUtility);

            //act 
            var result = (await scraper.DoSearch("https://www.google.com", "cricket", "india", 10)).ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
    }
}
