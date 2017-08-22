using RestSharp;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Framework.Movie;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Mocks
{
    internal class TestableTmdbManager : TmdbManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestableTmdbManager"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <param name="helper"></param>
        public TestableTmdbManager(IConfigurationManager configManager, IHelper helper) : base(configManager, helper)
        {
        }

        /// <summary>
        /// Executes the request asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// override for unit tests
        /// </remarks>
        protected override Task<IRestResponse> ExecuteRequestAsync(IRestRequest request)
        {
            IRestResponse response = new RestResponse();
            return Task.FromResult(response);
        }
    }
}
