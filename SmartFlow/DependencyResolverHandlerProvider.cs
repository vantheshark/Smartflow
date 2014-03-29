using System.Collections.Generic;
using Smartflow.Core.CQRS;

namespace Smartflow.Core
{
    internal class DependencyResolverHandlerProvider : IHandlerProvider
    {
        public IEnumerable<IHandler<T>> GetHandlers<T>() where T : class, IMessage
        {
            return DependencyResolver.Current.GetServices<IHandler<T>>();
        }
    }
}