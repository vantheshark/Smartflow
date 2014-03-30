using Autofac;
using log4net;
using Smartflow.Core.CQRS;
using Smartflow.Demo.BingNewsSearch;

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

            builder.RegisterType<BingNewsSearchCommandHandler>().As<IHandler<NewsSearchCommand>>().InstancePerDependency();

            builder.RegisterType<PublishNewsArticleHandler>().As<IHandler<NewsArticleFound>>().InstancePerDependency();
            
            builder.RegisterType<SaveNewsPostHandler>().As<IHandler<NewsArticleFound>>().InstancePerDependency();
        }
    }
}
