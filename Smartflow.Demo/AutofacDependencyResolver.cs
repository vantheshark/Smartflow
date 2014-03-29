using System;
using System.Collections.Generic;
using Autofac;
using Smartflow.Core;

namespace Smartflow.Demo
{
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public AutofacDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public T GetService<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        public IEnumerable<T> GetServices<T>() where T : class
        {
            return _container.Resolve<IEnumerable<T>>();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var svc =  _container.Resolve(serviceType);
            return svc != null ? new[] {svc} : new object[0];
        }
    }
}