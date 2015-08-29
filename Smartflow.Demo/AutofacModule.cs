using Autofac;
using log4net;
using Smartflow.Core.CQRS;
using Smartflow.Demo.BingNewsSearch;
using Smartflow.Demo.Common;

namespace Smartflow.Demo
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(LogManager.GetLogger("")).As<ILog>().SingleInstance();

            builder.RegisterType<BingNewsSearcher>().As<IBingNewsSearcher>().SingleInstance();

            builder.RegisterGeneric(typeof (InMemoryDatabase<>)).AsImplementedInterfaces();

            builder.RegisterInstance(InternalBus.Current).AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<SearchAuditService>().As<ISearchAuditService>().SingleInstance();

            builder.RegisterType<BingNewsSearchCommandHandler>().As<ICommandHandler<NewsSearchCommand>>().InstancePerDependency();

            builder.RegisterType<PublishNewsArticleHandler>().As<IEventHandler<NewsArticleFound>>().InstancePerDependency();

            builder.RegisterType<SaveNewsPostHandler>().As<IEventHandler<NewsArticleFound>>().InstancePerDependency();
        }
    }
}
