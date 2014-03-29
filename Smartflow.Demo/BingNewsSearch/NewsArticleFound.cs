using Smartflow.Core.CQRS;

namespace Smartflow.Demo.BingNewsSearch
{
    public class NewsArticleFound : Event
    {
        public SearchResult Article { get; set; }
    }
}
