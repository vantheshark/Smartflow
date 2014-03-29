using System.Collections.Generic;

namespace Smartflow.Demo.BingNewsSearch
{
    public interface ISearchAuditService
    {
        List<NewsSearchAudit> GetAll();
        void Audit(NewsSearchAudit audit);
    }
}