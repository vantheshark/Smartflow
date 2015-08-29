using Smartflow.Core.CQRS;

namespace Smartflow.Demo.Common
{
    public class NewsArticleFound : Event, IDistributedMessage
    {
        public SearchResult Article { get; set; }
    }
}
