using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace Search
{
    public class MySearch
    {
        private const string BrowserAgent = "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to My Search");
            Console.WriteLine("Enter 1 for Google Search, 2 for Bing Search");
            var choice = Console.ReadLine();

            Console.WriteLine("Enter your search query:");
            var query = Console.ReadLine();

            List<SearchResult> searchResults;

            if (choice == "1")
                searchResults = PerformGoogleSearch(query, 3);
            else if (choice == "2")
                searchResults = PerformBingSearch(query, 3);
            else
                return;

            ShowResults(searchResults);

            Console.ReadLine();
        }

        public static List<SearchResult> PerformGoogleSearch(string searchQuery, int numberOfPages)
        {
            var searchResults = new List<SearchResult>();

            string formattedQuery = Uri.EscapeDataString(searchQuery);

            for (var i = 1; i <= numberOfPages; i++)
            {
                var url = $"http://www.google.com/search?q={formattedQuery}&num=100&start={(i - 1) * 10}";
                searchResults.AddRange(GetResultsFromPage(url, "//div[@class='yuRUbf']"));
            }
            return searchResults;
        }

        public static List<SearchResult> PerformBingSearch(string searchQuery, int numberOfPages)
        {
            var searchResults = new List<SearchResult>();

            string formattedQuery = Uri.EscapeDataString(searchQuery);

            for (var i = 1; i <= numberOfPages; i++)
            {
                var url = $"https://www.bing.com/search?q={formattedQuery}&count=50&first={(i - 1) * 50 + 1}";
                searchResults.AddRange(GetResultsFromPage(url, "//li[@class='b_algo']"));
            }
            return searchResults;
        }

        private static IEnumerable<SearchResult> GetResultsFromPage(string url, string xpath)
        {
            var web = new HtmlWeb { UserAgent = BrowserAgent };
            var htmlDocument = web.Load(url);
            var htmlNodes = htmlDocument.DocumentNode.SelectNodes(xpath);

            if (htmlNodes == null)
            {
                Console.WriteLine($"No results found at {url} with xpath {xpath}");
                return Enumerable.Empty<SearchResult>();
            }

            return htmlNodes.Select(tag => new SearchResult
            {
                PageUrl = tag.Descendants("a").FirstOrDefault()?.Attributes["href"].Value,
                PageTitle = WebUtility.HtmlDecode(tag.Descendants("h2").FirstOrDefault()?.InnerText)
            });
        }

        public static void ShowResults(List<SearchResult> searchResults)
        {
            foreach (var result in searchResults)
            {
                Console.WriteLine(result.PageTitle);
                Console.WriteLine(result.PageUrl + "\n");
            }
        }
    }
}
