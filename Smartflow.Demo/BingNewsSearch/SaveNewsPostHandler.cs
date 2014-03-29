using Smartflow.Core.CQRS;

namespace Smartflow.Demo.BingNewsSearch
{
    public class SaveNewsPostHandler : Handler<NewsArticleFound>
    {
        private readonly IRepository<NewsArticleFound> _store;

        public SaveNewsPostHandler(IRepository<NewsArticleFound> store)
        {
            _store = store;
        }

        public override void Handle(NewsArticleFound message)
        {
            _store.Save(message);
        }
    }
}