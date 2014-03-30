using System;
using System.Collections;
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
            try
            {
                return _container.Resolve<T>();
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public IEnumerable<T> GetServices<T>() where T : class
        {
            return _container.Resolve<IEnumerable<T>>();
        }

        private static readonly Type EnumerableHandlers = typeof(IEnumerable<>);
        public IEnumerable<object> GetServices(Type serviceType)
        {
            IEnumerable svcs;
            try
            {
                var types = EnumerableHandlers.MakeGenericType(serviceType);
                svcs = _container.Resolve(types) as IEnumerable;
                if (svcs == null)
                {
                    yield break;
                }
            }
            catch (Exception)
            {
                yield break;
            }
                
            foreach(var s in svcs)
            {
                yield return s;
            }
        }
    }
}