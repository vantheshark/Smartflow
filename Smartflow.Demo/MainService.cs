using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Smartflow.Core.CQRS;
using Smartflow.Demo.BingNewsSearch;
using Smartflow.Demo.Common;

namespace Smartflow.Demo
{
    public partial class MainService : ServiceBase
    {
        private readonly ICommandSender _commandSender;
        private readonly ISearchAuditService _auditService;
        private Timer _timer;
        public MainService(ICommandSender commandSender, ISearchAuditService auditService)
        {
            _commandSender = commandSender;
            _auditService = auditService;
            InitializeComponent();
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            _timer = new Timer(_ =>
            {
                var trackingQuery = new [] {"#MH370", "Tony Abbot", "Obama", "Putin", "Ukraina"};

                var allAudit = _auditService.GetAll();

                var commands = trackingQuery.GroupJoin(allAudit, x => x, a => a.Keyword, (x,a) => new NewsSearchCommand
                {
                    LatestArticleDate = a.DefaultIfEmpty().FirstOrDefault() != null 
                                      ? a.DefaultIfEmpty().First().LastArticleDate 
                                      : DateTime.MinValue,
                    Query = x
                }).ToList();
                commands.ForEach(c => _commandSender.Send(c));
            });
            _timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromHours(20));
        }

        protected override void OnStop()
        {
            _timer.Dispose();
        }
    }
}
