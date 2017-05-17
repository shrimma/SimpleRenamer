using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// DependencyInjectionContext Interface
    /// </summary>
    public interface IDependencyInjectionContext
    {
        /// <summary>
        /// Initializes the dependency injection context
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service</typeparam>
        /// <returns>The service if it exists</returns>
        T GetService<T>();

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service</typeparam>
        /// <param name="constructorArguments">List of key value pair for any constructor arguments - Key is argument name and value is argument value</param>
        /// <returns>The service if it exists</returns>
        T GetService<T>(List<KeyValuePair<string, object>> constructorArguments);

        /// <summary>
        /// Bind a context to a constant
        /// </summary>
        /// <typeparam name="T">Type to bind to</typeparam>
        /// <param name="context">Context to use for the constant binding</param>
        void BindConstant<T>(T context);

        /// <summary>
        /// Releases the service object
        /// </summary>
        /// <param name="serviceToRelease">The service to release</param>
        void ReleaseService(object serviceToRelease);
    }
}
