using System.Threading.Tasks;
using Smartflow.Core.CQRS;

namespace Smartflow.Demo.Common
{
    public class SaveNewsPostHandler : AsyncEventHandler<NewsArticleFound>
    {
        private readonly IRepository<NewsArticleFound> _store;

        public SaveNewsPostHandler(IRepository<NewsArticleFound> store)
        {
            _store = store;
        }

        /// <summary>
        /// This method to make sure the async event handler works
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override Task HandleAsync(NewsArticleFound message)
        {
            _store.Save(message);
            return Task.FromResult(false);
        }
    }
}