using LightInject;
using LightInject.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace PI.Service
{
    public class LightInjectResolver : IDependencyResolver
    {
        private readonly IServiceContainer serviceContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightInjectWebApiDependencyResolver"/> class.
        /// </summary>
        /// <param name="serviceContainer">The <see cref="IServiceContainer"/> instance to 
        /// be used for resolving service instances.</param>
        internal LightInjectResolver(IServiceContainer serviceContainer)
        {
            this.serviceContainer = serviceContainer;
        }

        /// <summary>
        /// Disposes the underlying <see cref="IServiceContainer"/>.
        /// </summary>
        public void Dispose()
        {
            serviceContainer.Dispose();
        }

        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <returns>The requested service instance if available, otherwise null.</returns>                
        public object GetService(Type serviceType)
        {
            return serviceContainer.TryGetInstance(serviceType);
        }

        /// <summary>
        /// Gets all instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of services to resolve.</param>
        /// <returns>A list that contains all implementations of the <paramref name="serviceType"/>.</returns>                
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return serviceContainer.GetAllInstances(serviceType);
        }

        /// <summary>
        /// Starts a new <see cref="IDependencyScope"/> that represents 
        /// the scope for services registered with <see cref="PerScopeLifetime"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="IDependencyScope"/>.
        /// </returns>
        public IDependencyScope BeginScope()
        {
            return new LightInjectWebApiDependencyScope(serviceContainer, serviceContainer.BeginScope());
        }
    }
}