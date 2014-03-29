using log4net;
using Smartflow.Core.CQRS;

namespace Smartflow.Demo.BingNewsSearch
{
    /// <summary>
    /// Publish the post to websocket to display realtime search
    /// </summary>
    public class PublishNewsArticleHandler : Handler<NewsArticleFound>
    {
        private readonly ILog _logger;

        public PublishNewsArticleHandler(ILog logger)
        {
            _logger = logger;
        }

        public override void Handle(NewsArticleFound message)
        {
            if (message.Article.Description.Length > 100)
            {
                message.Article.Description = message.Article.Description.Substring(0, 100) + " ...";
            }

            _logger.Info(string.Format("{0}:{1}\n{2}\n\n", message.Article.PublishDate, message.Article.Title, message.Article.Description));
        }
    }
}