using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using Autofac;
using Smartflow.Core;
using Smartflow.Core.CQRS;
using Smartflow.Demo.BingNewsSearch;
using Smartflow.Demo.Common;
using Smartflow.RabbitMQ;

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
            log4net.Config.XmlConfigurator.Configure();

            var service = BasicInit();
            //var service = InitWithAutofacDependencyInjection();
            //var service = InitWithRabbitMqAndAutofac();
            
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

        /// <summary>
        /// This demo method will use InternalBus to send/receive message
        /// </summary>
        /// <returns></returns>
        private static MainService BasicInit()
        {
            GlobalFilters.Filters.Add(new ConsoleLogMessageFilter());

            var logger = log4net.LogManager.GetLogger(typeof(Program));

            HandlerProvider.Providers.RegisterCommandHandler(new BingNewsSearchCommandHandler(new SearchAuditService(new InMemoryDatabase<NewsSearchAudit>()), 
                                                                                       new BingNewsSearcher(logger), 
                                                                                       logger));

            HandlerProvider.Providers.RegisterEventHandler(new SaveNewsPostHandler(new InMemoryDatabase<NewsArticleFound>()));

            HandlerProvider.Providers.RegisterEventHandler(new PublishNewsArticleHandler(logger));

            return new MainService(InternalBus.Current, new SearchAuditService(new InMemoryDatabase<NewsSearchAudit>()));
        }

        /// <summary>
        /// This demo method will use Autofac to resolve all dependencies and handlers
        /// </summary>
        /// <returns></returns>
        private static MainService InitWithAutofacDependencyInjection()
        {
            GlobalFilters.Filters.Add(new ConsoleLogMessageFilter());

            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule());
            builder.RegisterType<MainService>().AsSelf().SingleInstance();

            _container = builder.Build();
            DependencyResolver.SetDependencyResolver(new AutofacDependencyResolver(_container));
            
            return _container.Resolve<MainService>();

        }

        /// <summary>
        /// Make sure you have rabbitMQ setup on localhost to use this demo method
        /// </summary>
        /// <returns></returns>
        private static MainService InitWithRabbitMqAndAutofac()
        {
            GlobalFilters.Filters.Add(new ConsoleLogMessageFilter());

            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule());
            builder.RegisterType<MainService>().AsSelf().SingleInstance();

            var rabbitMqModule = new RabbitMqModule().WithConfig(ConfigurationManager.AppSettings);
            rabbitMqModule.Load(new LogicalPriorityMapperDemo()).Start();
            builder.RegisterInstance(InternalBus.Current).AsImplementedInterfaces().SingleInstance();

            _container = builder.Build();
            DependencyResolver.SetDependencyResolver(new AutofacDependencyResolver(_container));

            return _container.Resolve<MainService>();
        }
    }
}
