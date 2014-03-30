using Smartflow.Core.CQRS;

namespace Smartflow.Demo.BingNewsSearch
{
    public class NewsArticleFound : Event, IDistributedMessage
    {
        public SearchResult Article { get; set; }
    }
}
