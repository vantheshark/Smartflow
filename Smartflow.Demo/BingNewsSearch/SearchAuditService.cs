using System.Collections.Generic;
using System.Linq;


namespace Smartflow.Demo.BingNewsSearch
{
    public class SearchAuditService : ISearchAuditService
    {
        private readonly IRepository<NewsSearchAudit> _store;
        private readonly object _sync = new object();
        public SearchAuditService(IRepository<NewsSearchAudit> store)
        {
            _store = store;
        }

        public List<NewsSearchAudit> GetAll()
        {
            return _store.GetAll().ToList();
        }

        public void Audit(NewsSearchAudit audit)
        {
            lock (_sync)
            {
                var all = GetAll();
                var item = all.FirstOrDefault(x => x.Keyword == audit.Keyword);
                if (item == null)
                {
                    _store.Save(audit);
                }
                else if (item.LastArticleDate < audit.LastArticleDate)
                {
                    item.LastArticleDate = audit.LastArticleDate;
                }
            }
        }
    }
}