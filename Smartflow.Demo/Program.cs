using System;
using System.ServiceProcess;
using System.Threading;
using Autofac;
using Smartflow.Core;
using Smartflow.Demo.BingNewsSearch;

namespace Smartflow.Demo
{
    static class Program
    {
        private static IContainer _container;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Init();
            var service = _container.Resolve<MainService>();
            if (!Environment.UserInteractive)
            {
                ServiceBase.Run(service);
            }
            else
            {
                service.Start(null);
                Thread.Sleep(Timeout.Infinite);
                service.Stop();
            }
        }

        private static void Init()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule());
            builder.RegisterType<MainService>().AsSelf().SingleInstance();

            _container = builder.Build();
            DependencyResolver.SetDependencyResolver(new AutofacDependencyResolver(_container));
            GlobalFilters.Filters.Add(new ConsoleLogMessageFilter());

            //var logger = log4net.LogManager.GetLogger("");
            //HandlerProvider.Providers.RegisterHandler(new BindNewsSearchCommandHandler(new SearchAuditService(new InMemoryDatabase<NewsSearchAudit>()), new BingNewsSearcher(logger), logger));
        }
    }
}
