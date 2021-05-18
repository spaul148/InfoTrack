using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace InfoTrack.Framework.Tests
{
    [TestClass]
    public class HtmlParserTest
    {
        private string _html;
        private GoogleHtmlParser _parser;
        
        [TestInitialize]
        public void Init()
        {
            _html = File.ReadAllText("Sample2.Html");
            _parser = new GoogleHtmlParser();
        }

        [TestMethod]
        public void TestParsingWithId()
        {
            //Act
            var parsed = _parser.ParseTagWithId(_html, "div", "main");

            //Assert
            Assert.IsNotNull(parsed);
            Assert.AreEqual(110498, parsed.Length);
        }

        [TestMethod]
        public void TestParsingWithElement()
        {
            //Act
            var parsed = _parser.ParseTagWithId(_html, "div", "main");
            var h3 = _parser.ParseTag(parsed, "h3");
            
            //Assert
            Assert.IsNotNull(parsed);
            Assert.IsNotNull(h3);
        }

        [TestMethod]
        public void TestFindingParentTag()
        {
            //Act
            var parsed = _parser.ParseTagWithId(_html, "div", "main");
            var h3 = _parser.ParseTag(parsed, "h3");
            var result = _parser.CheckParentTag(h3, _html, "a", new string[] { "<b" }, 1);

            //Assert
            Assert.IsNotNull(parsed);
            Assert.IsNotNull(h3);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestParsingParentTag()
        {
            //Act
            var parsed = _parser.ParseTagWithId(_html, "div", "main");
            var h3 = _parser.ParseTag(parsed, "h3");
            var result = _parser.FindParentTag(h3, _html, "a", new string[] { "<b" }, 1);

            //Assert
            Assert.IsNotNull(parsed);
            Assert.IsNotNull(h3);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGettingAttribute()
        {
            //Act
            var parsed = _parser.ParseTagWithId(_html, "div", "main");
            var h3 = _parser.ParseTag(parsed, "h3");
            var result = _parser.FindParentTag(h3, _html, "a", new string[] { "<b" }, 1);
            var url = _parser.GetAttributeValue(result, "a", "href");

            //Assert
            Assert.IsNotNull(parsed);
            Assert.IsNotNull(h3);
            Assert.IsNotNull(result);
            Assert.IsNotNull(url);
        }

        [TestMethod]
        public void TestParsingsAllTags()
        {
            //Act
            var parsed = _parser.ParseTagWithId(_html, "div", "main");
            var h3 = _parser.ParseAllTags(parsed, "h3");

            //Assert
            Assert.IsNotNull(parsed);
            Assert.IsNotNull(h3);
            Assert.AreEqual(100, h3.Count);
        }

        [TestMethod]
        public void TestFindingSibling()
        {
            //Act
            var parsed = _parser.ParseTagWithId(_html, "div", "main");
            var h3 = _parser.ParseTag(parsed, "h3");
            var result = _parser.FindParentTag(h3, _html, "div", new string[] { "<b" }, 2);
            var sibling = _parser.FindNextSiblingTag(result, _html, "div", 2);

            //Assert
            Assert.IsNotNull(parsed);
            Assert.IsNotNull(h3);
            Assert.IsNotNull(result);
            Assert.IsNotNull(sibling);
        }

        [TestMethod]
        public void TestRemovingAllHtmlTags()
        {
            //Act
            var parsed = _parser.ParseTagWithId(_html, "div", "main");
            var h3 = _parser.ParseTag(parsed, "h3");
            var clean = _parser.RemoveAllHtmlTags(h3);

            //Assert
            Assert.IsNotNull(parsed);
            Assert.IsFalse(clean.Contains("<"));
        }
    }
}
