using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrack.Framework
{
    public class GoogleHtmlParser : IHtmlParser
    {
        private const string _greaterThan = ">";
        private const string _lessThan = "<";

        public GoogleHtmlParser() { }

        /// <summary>
        /// Parses and returns a first match html element that matches the passed in tag name
        /// </summary>
        /// <param name="html">html content to search</param>
        /// <param name="tag">name of the html element to search</param>
        /// <param name="startIndex">index to start searching from inside html</param>
        /// <returns>parsed html element</returns>
        public string ParseTag(string html, string tag, int startIndex = 0)
        {
            var tupleResult = GetStartTagOfElement(html, tag, startIndex);
            if (tupleResult == null || tupleResult.Item1 == null) return null;

            var completeElement = GrabHtmlElementFromStartToEnd(html, tag, tupleResult.Item2);

            return completeElement;

        }

        /// <summary>
        /// Parses and returns all html elements that match the passed in tag name
        /// </summary>
        /// <param name="html">html content to search</param>
        /// <param name="tag">name of the html element to search</param>
        /// <param name="startIndex">index to start searching from inside html</param>
        /// <returns>parsed html elements</returns>
        public List<HtmlParserResult> ParseAllTags(string html, string tag, int startIndex = 0)
        {
            List<HtmlParserResult> tags = new List<HtmlParserResult>();
            var startTag = FormatStartTag(tag);
            int idx = 0;
            int counter = 1;

            while (idx != -1)
            {
                var tupleResult = GetStartTagOfElement(html, tag, startIndex);
                if (tupleResult == null || tupleResult.Item1 == null) return tags;

                idx = tupleResult.Item2;
                if (idx == -1) return tags;

                var completeElement = GrabHtmlElementFromStartToEnd(html, tag, idx);
                if (completeElement != null)
                {
                    var result = new HtmlParserResult(counter, completeElement);
                    tags.Add(result);
                    startIndex = idx + completeElement.Length;
                }
                counter++;
            }

            return tags;
        }

        /// <summary>
        /// Parses and returns an html element that matches the passed in tag name and whose id matches passed in id value
        /// for a more precise match
        /// </summary>
        /// <param name="html">html content to search inside</param>
        /// <param name="tag">tag name of the html element to find</param>
        /// <param name="id">id value of the element to search for a precise match</param>
        /// <param name="attributeValue">attribute value to search for inside the element</param>
        /// <returns>parsed html element</returns>
        public string ParseTagWithId(string inputHtml, string tag, string id)
        {
            string idMatch = $"id=\"{id}\"";
            var startTag = FormatStartTag(tag);

            var tupleResult = GetStartTagOfElement(inputHtml, tag, html => html.Contains(idMatch));
            if (tupleResult == null || tupleResult.Item1 == null) return null;

            var completeElement = GrabHtmlElementFromStartToEnd(inputHtml, tag, tupleResult.Item2);

            return completeElement;

        }

        /// <summary>
        /// Parses and returns an html element that matches the passed in tag name and whose attribute name and value match passed in values
        /// for a more precise match
        /// </summary>
        /// <param name="html">html content to search inside</param>
        /// <param name="tag">tag name of the html element to find</param>
        /// <param name="attribute">name of the attribute to search on element</param>
        /// <param name="attributeValue">attribute value to search for inside the element</param>
        /// <returns>Parsed html element</returns>
        public string ParseTagWithAttribute(string html, string tag, string attribute, string attributeValue)
        {
            string attrbuteToMatch = !string.IsNullOrWhiteSpace(attributeValue) ? $"{attribute}=\"{attributeValue}\"" : $"{attribute}=\"";

            var startTag = FormatStartTag(tag);

            var tupleResult = GetStartTagOfElement(html, tag, html => html.Contains(attrbuteToMatch));
            if (tupleResult == null || tupleResult.Item1 == null) return null;

            var completeElement = GrabHtmlElementFromStartToEnd(html, tag, tupleResult.Item2);

            return completeElement;

        }

        /// <summary>
        /// Exhaustively finds parent tag of an element
        /// </summary>
        /// <param name="htmlElement">child element whose parent needs finding</param>
        /// <param name="html">root html or main html to search</param>
        /// <param name="tagToMatch">parent tag to match</param>
        /// <returns></returns>
        public string FindParentTag(string html, string htmlElement, string tagToMatch)
        {
            int? currentIndex = FindParentIndex(htmlElement, html, tagToMatch, Array.Empty<string>(), 100);

            //now go grab entire parent element
            if (currentIndex == null || currentIndex == -1) return null;

            return ParseTag(html, tagToMatch, currentIndex.Value);
        }

        /// <summary>
        /// Finds parent tag of an element using number of levels to check
        /// </summary>
        /// <param name="htmlElement">child element whose parent needs finding</param>
        /// <param name="html">root html or main html to search</param>
        /// <param name="tagToMatch">parent tag to match</param>
        /// <param name="elementsToIgnore">elements to ignore while doing the parent search</param>
        /// <param name="levelsToCheck">number of ancenstral levels to search</param>
        /// <returns></returns>
        public string FindParentTag(string htmlElement, string html, string tagToMatch, string[] elementsToIgnore, int levelsToCheck = 1)
        {
            int? currentIndex = FindParentIndex(htmlElement, html, tagToMatch, elementsToIgnore, levelsToCheck);

            //now go grab entire parent element
            if (currentIndex == null || currentIndex == -1) return null;

            return ParseTag(html, tagToMatch, currentIndex.Value);
        }

        /// <summary>
        /// Finds next sibling element of current html element
        /// </summary>
        /// <param name="htmlElement">element whose next sibling needs finding</param>
        /// <param name="html">source html</param>
        /// <param name="tagToMatch">sibling tag to match on</param>
        /// <returns>found sibling or null</returns>
        public string FindNextSiblingTag(string htmlElement, string html, string tagToMatch, int level = 1)
        {
            int currentLevel = 1;
            int startIndex = html.IndexOf(htmlElement) + htmlElement.Length;

            while (currentLevel < level)
            {
                var sibling = ParseTag(html, tagToMatch, startIndex);
                startIndex = html.IndexOf(sibling, startIndex) + sibling.Length;
                currentLevel++;
            }

            return ParseTag(html, tagToMatch, startIndex);
        }

        /// <summary>
        /// Removes all html tags from the html
        /// </summary>
        /// <param name="html">html whose tags need removed</param>
        /// <returns>clean text without html tags</returns>
        public string RemoveAllHtmlTags(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return null;

            int startIndex = html.IndexOf(_lessThan);
            while (startIndex != -1)
            {
                var endIndex = html.IndexOf(_greaterThan, startIndex + 1);
                html = html.Remove(startIndex, endIndex - startIndex + 1);
                startIndex = html.IndexOf(_lessThan);
            }
            return html;
        }

        /// <summary>
        /// Checks for parent tag of an element using number of levels to check
        /// </summary>
        /// <param name="htmlElement">child element whose parent needs finding</param>
        /// <param name="html">root html or main html to search</param>
        /// <param name="tagToMatch">parent tag to match</param>
        /// <param name="elementsToIgnore">elements to ignore while doing the parent search</param>
        /// <param name="levelsToCheck">number of ancenstral levels to search</param>
        /// <returns></returns>
        public bool CheckParentTag(string htmlElement, string html, string tagToMatch, string[] elementsToIgnore, int levelsToCheck = 1)
        {
            int? currentIndex = FindParentIndex(htmlElement, html, tagToMatch, elementsToIgnore, levelsToCheck);

            //now go grab entire parent element
            if (currentIndex == null || currentIndex == -1) return false;

            return true;
        }

        /// <summary>
        /// Grabs attribute value from an html element
        /// </summary>
        /// <param name="html">Html of the element</param>
        /// <param name="tag">tag name of the element</param>
        /// <param name="attribute">attribute name</param>
        /// <returns>Parsed attribute value</returns>
        public string GetAttributeValue(string html, string tag, string attribute)
        {
            var tuple = GetStartTagOfElement(html, tag, 0);
            var startTag = tuple.Item1;
            var attributeToFind = $"{attribute}=\"";

            var attributeStartIndex = startTag.IndexOf(attributeToFind);
            if (attributeStartIndex == -1) return null;
            attributeStartIndex = attributeStartIndex + attributeToFind.Length;
            var attributeEndIndex = startTag.IndexOf("\"", attributeStartIndex);
            var attributeValue = startTag.Substring(attributeStartIndex, attributeEndIndex - attributeStartIndex);
            attributeValue = attributeValue.Replace("\"", "");
            return attributeValue;
        }

        private Tuple<string, int> GetStartTagOfElement(string html, string tag, int startIndex)
        {
            string startElementHtml;
            int startTagIndex;
            var startTag = FormatStartTag(tag);

            //jump to tag name
            startTagIndex = html.IndexOf(startTag, startIndex);
            if (!ValidPointer(startTagIndex)) return null;

            var endStartTagIndex = html.IndexOf(_greaterThan, startTagIndex + 1);
            if (!ValidPointer(endStartTagIndex)) return null;

            var startTagLength = endStartTagIndex - startTagIndex + 1;

            startElementHtml = html.Substring(startTagIndex, startTagLength);

            return Tuple.Create(startElementHtml, startTagIndex);
        }

        private Tuple<string, int> GetStartTagOfElement(string html, string tag, Func<string, bool> matchCriteria)
        {
            var startTag = FormatStartTag(tag);
            bool found = false;
            string startElementHtml = null;
            int toStartFromIndex = 0;
            int startTagIndex = 0;

            while (!found)
            {
                //jump to tag name
                startTagIndex = html.IndexOf(startTag, toStartFromIndex);
                if (!ValidPointer(startTagIndex)) return null;

                var endStartTagIndex = html.IndexOf(_greaterThan, startTagIndex + 1);
                if (!ValidPointer(endStartTagIndex)) return null;

                var startTagLength = endStartTagIndex - startTagIndex + 1;

                startElementHtml = html.Substring(startTagIndex, startTagLength);

                if (!matchCriteria(startElementHtml))
                {
                    toStartFromIndex = startTagIndex + startTagLength;
                }
                else
                {
                    found = true;
                }
            }

            return Tuple.Create(startElementHtml, startTagIndex);
        }

        private static int? FindParentIndex(string htmlElement, string html, string tagToMatch, string[] elementsToIgnore, int levelsToCheck = 1)
        {
            var startTag = FormatStartTag(tagToMatch);
            var foundParentTag = "";
            var currentIndex = html.IndexOf(htmlElement);
            int currentLevel = 1;


            while (foundParentTag != startTag)
            {
                if (currentIndex == -1 || currentLevel > levelsToCheck) return null;

                var parentStartTagIdx = html.LastIndexOf(_lessThan, currentIndex - 1);
                if (parentStartTagIdx == -1) return null;

                //find the space or >, whichever comes first e.g. </p> or <p style=
                var endIndex = Math.Min(html.IndexOf(_greaterThan, parentStartTagIdx), html.IndexOf(" ", parentStartTagIdx));

                foundParentTag = html.Substring(parentStartTagIdx, (endIndex - parentStartTagIdx)).Trim();
                if (elementsToIgnore.Contains(foundParentTag))
                {
                    currentIndex = parentStartTagIdx;
                    continue;
                }

                if (foundParentTag.Contains($"/"))
                {
                    var toIgnoreTag = foundParentTag.Replace("/","");
                    currentIndex = html.LastIndexOf(toIgnoreTag, parentStartTagIdx - 1);
                    continue;
                }

                if (foundParentTag != startTag)
                {
                    currentLevel++;
                }

                currentIndex = parentStartTagIdx;
            }

            return currentIndex;
        }

        private string GrabHtmlElementFromStartToEnd(string html, string tag, int start)
        {
            int endIndex = 0;
            int startIndex = start;
            var startTag = FormatStartTag(tag);
            var endTag = GetEndTag(tag);

            while (endIndex != -1)
            {
                endIndex = html.IndexOf(endTag, startIndex);
                if (endIndex == -1) return null;

                var innerHtml = html.Substring(start, endIndex - start + endTag.Length);

                var howManyNestedStartTags = innerHtml.Split(startTag).Length;
                var howManyNestedEndTags = innerHtml.Split(endTag).Length;

                if (howManyNestedEndTags >= howManyNestedStartTags)
                {
                    return innerHtml;
                }
                startIndex = endIndex + endTag.Length;
            }
            return null;
        }

        private static string FormatStartTag(string tagName)
        {
            return $"{_lessThan}{tagName}";
        }

        private static string GetEndTag(string tagName)
        {
            return $"{_lessThan}/{tagName}{_greaterThan}";
        }



        private static bool ValidPointer(int pointer)
        {
            if (pointer == -1) return false;

            return true;
        }

    }
}
