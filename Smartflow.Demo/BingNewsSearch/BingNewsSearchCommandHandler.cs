using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smartflow.Core.CQRS;
using log4net;
using Smartflow.Demo.Common;

namespace Smartflow.Demo.BingNewsSearch
{
    /// <summary>
    /// Search all public posts that have photo and specific #hashtag
    /// </summary>
    [HandlerPerformanceLogFilter]
    public class BingNewsSearchCommandHandler : AsyncCommandHandler<NewsSearchCommand>
    {
        private readonly ISearchAuditService _auditService;
        private readonly IBingNewsSearcher _searcher;
        private readonly ILog _logger;

        public BingNewsSearchCommandHandler(ISearchAuditService auditService, IBingNewsSearcher searcher, ILog logger)
        {
            _auditService = auditService;
            _searcher = searcher;
            _logger = logger;
        }

        public override async Task<IEnumerable<Event>> HandleAsync(NewsSearchCommand message)
        {
            // Search
            var result = await _searcher.SearchAsync(message.Query);
            var events = new List<Event>();
            if (result != null && result.Data != null && result.Data.Length > 0)
            {
                if (message.LatestArticleDate > DateTime.MinValue)
                {
                    var data = result.Data.ToList();
                    data.RemoveAll(d => d.PublishDate <= message.LatestArticleDate);
                    result.Data = data.ToArray();
                }
                
                if (result.Data.Length == 0)
                {
                    _logger.InfoFormat("No new result found for {0}", message.Query);
                }

                foreach (var p in result.Data)
                {
                    // Publish post found
                    events.Add(new NewsArticleFound
                    {
                        Article = p,
                        Priority = message.Priority + 1
                    });
                }

                // Audit
                if (result.Data.Length > 0)
                {
                    _auditService.Audit(new NewsSearchAudit
                    {
                        Keyword = message.Query,
                        LastArticleDate = result.Data.Max(x => x.PublishDate)
                    });
                }
            }
            return events;
        }
    }
}