using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using ProductCustomerService.BusinessLayer;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace ProductCustomerService
{

    //public interface IDependencyResolver : IDependencyScope, IDisposable
    //{
    //    IDependencyScope BeginScope();
    //}

    //public interface IDependencyScope : IDisposable
    //{
    //    object GetService(Type serviceType);
    //    IEnumerable<object> GetServices(Type serviceType);
    //}

    public class UnityResolver : IDependencyResolver
    {
        protected IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //var container = new UnityContainer();
            //container.RegisterType<IBusinessProvider, BusinessProvider>(new HierarchicalLifetimeManager());
            //config.DependencyResolver = new UnityResolver(container);

            // Other Web API configuration not shown.
        }
    }
}
