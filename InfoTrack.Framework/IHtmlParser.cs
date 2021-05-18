using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrack.Framework
{
    public interface IHtmlParser
    {
        string ParseTag(string html, string tag, int startIndex = 0);
        List<HtmlParserResult> ParseAllTags(string html, string tag, int startIndex = 0);
        string ParseTagWithId(string inputHtml, string tag, string id);
        string ParseTagWithAttribute(string html, string tag, string attribute, string attributeValue);
        string FindParentTag(string html, string htmlElement, string tagToMatch);
        string FindParentTag(string htmlElement, string html, string tagToMatch, string[] elementsToIgnore, int levelsToCheck = 1);
        bool CheckParentTag(string htmlElement, string html, string tagToMatch, string[] elementsToIgnore, int levelsToCheck = 1);
        string GetAttributeValue(string html, string tag, string attribute);
        string FindNextSiblingTag(string htmlElement, string html, string tagToMatch, int level = 1);
        string RemoveAllHtmlTags(string html);
    }
}
