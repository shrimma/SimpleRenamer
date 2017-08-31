using Ninject;
using Ninject.Parameters;
using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sarjee.SimpleRenamer.DependencyInjection
{
    /// <summary>
    /// Dependency Injection Context
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IDependencyInjectionContext" />
    /// <seealso cref="System.IDisposable" />
    public class DependencyInjectionContext : IDependencyInjectionContext, IDisposable
    {
        private IKernel _kernel;

        /// <summary>
        /// Starts the process of setting up dependency injection
        /// </summary>
        public void Initialize()
        {
            // Order of the modules is important.
            _kernel = new StandardKernel();
            _kernel.Load(Assembly.GetExecutingAssembly());
            _kernel.Bind<IDependencyInjectionContext>().ToConstant<DependencyInjectionContext>(this);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service</typeparam>
        /// <returns>
        /// The service if it exists
        /// </returns>
        /// <inheritdoc />
        public T GetService<T>()
        {
            return _kernel.Get<T>();
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service</typeparam>
        /// <param name="constructorArguments">List of key value pair for any constructor arguments - Key is argument name and value is argument value</param>
        /// <returns>
        /// The service if it exists
        /// </returns>
        /// <inheritdoc />
        public T GetService<T>(List<KeyValuePair<string, object>> constructorArguments)
        {
            List<ConstructorArgument> arguments = new List<ConstructorArgument>();
            foreach (var param in constructorArguments)
            {
                arguments.Add(new ConstructorArgument(param.Key, param.Value));
            }

            return _kernel.Get<T>(arguments.ToArray());
        }

        /// <summary>
        /// Bind a context to a constant
        /// </summary>
        /// <typeparam name="T">Type to bind to</typeparam>
        /// <param name="context">Context to use for the constant binding</param>
        public void BindConstant<T>(T context)
        {
            _kernel.Bind<T>().ToConstant(context);
        }

        /// <summary>
        /// Releases the service object
        /// </summary>
        /// <param name="serviceToRelease">The service to release</param>
        /// <exception cref="System.ArgumentNullException">serviceToRelease</exception>
        /// <inheritdoc />
        public void ReleaseService(object serviceToRelease)
        {
            if (serviceToRelease == null)
            {
                throw new ArgumentNullException("serviceToRelease");
            }

            _kernel.Release(serviceToRelease);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_kernel != null)
                    {
                        _kernel.Dispose();
                        _kernel = null;
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion        
    }
}
