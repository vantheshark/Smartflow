using System.Collections.Generic;

namespace Smartflow.Demo.Common
{
    public interface ISearchAuditService
    {
        List<NewsSearchAudit> GetAll();
        void Audit(NewsSearchAudit audit);
    }
}