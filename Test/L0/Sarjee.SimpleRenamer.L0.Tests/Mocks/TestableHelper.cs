using RestSharp;
using Sarjee.SimpleRenamer.Common;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.L0.Tests.Mocks
{
    internal class TestableHelper : Helper
    {
        private const string _loginResponse = "{\"token\":\"jwtToken\"}";

        public TestableHelper()
        {
        }

        protected override Task<IRestResponse> ExecuteRequestAsync(IRestClient restClient, IRestRequest request)
        {
            IRestResponse response = new RestResponse
            {
                StatusCode = HttpStatusCode.OK,
                Content = _loginResponse
            };

            return Task.FromResult(response);
        }
    }

    internal class ErrorCodeTestableHelper : Helper
    {
        public ErrorCodeTestableHelper()
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
        protected override Task<IRestResponse> ExecuteRequestAsync(IRestClient restClient, IRestRequest request)
        {
            IRestResponse response = new RestResponse
            {
                StatusCode = (HttpStatusCode)408,
            };
            return Task.FromResult(response);
        }
    }

    internal class WebExceptionTestableHelper : Helper
    {
        public WebExceptionTestableHelper()
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
        protected override Task<IRestResponse> ExecuteRequestAsync(IRestClient restClient, IRestRequest request)
        {
            throw new WebException();
        }
    }

    internal class ErrorExceptionTestableHelper : Helper
    {
        public ErrorExceptionTestableHelper()
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
        protected override Task<IRestResponse> ExecuteRequestAsync(IRestClient restClient, IRestRequest request)
        {
            IRestResponse response = new RestResponse
            {
                ErrorException = new ArgumentNullException("dummy")
            };
            return Task.FromResult(response);
        }
    }

    internal class UnauthorizedTestableHelper : Helper
    {
        public UnauthorizedTestableHelper()
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
        protected override Task<IRestResponse> ExecuteRequestAsync(IRestClient restClient, IRestRequest request)
        {
            IRestResponse response = new RestResponse
            {
                StatusCode = HttpStatusCode.Unauthorized
            };
            return Task.FromResult(response);
        }
    }
}
