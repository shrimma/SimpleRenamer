using Ninject;
using SimpleRenamer.Framework.Interface;
using System;
using System.Reflection;

namespace SimpleRenamer.DependencyInjection
{
    public class DependencyInjectionContext : IDependencyInjectionContext
    {
        private IKernel _kernel;

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

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

        /// <inheritdoc />
        public T GetService<T>()
        {
            return _kernel.Get<T>();
        }

        public void BindConstant<T>(T context)
        {
            _kernel.Bind<T>().ToConstant(context);
        }

        /// <inheritdoc />
        public void ReleaseService(object serviceToRelease)
        {
            if (serviceToRelease == null)
            {
                throw new ArgumentNullException("serviceToRelease");
            }

            _kernel.Release(serviceToRelease);
        }

        /// <inheritdoc />
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_kernel != null)
                {
                    _kernel.Dispose();

                    _kernel = null;
                }
            }
        }
    }
}
