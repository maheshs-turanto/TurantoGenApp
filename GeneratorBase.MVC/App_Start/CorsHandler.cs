using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.App_Start
{
/// <summary>The CORS handler.</summary>
public class CorsHandler : DelegatingHandler
{
    /// <summary>The origin.</summary>
    private const string Origin = "Origin";
    /// <summary>The access control request method.</summary>
    private const string AccessControlRequestMethod = "Access-Control-Request-Method";
    /// <summary>The access control request headers.</summary>
    private const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
    /// <summary>The access control allow origin.</summary>
    private const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
    /// <summary>The access control allow methods.</summary>
    private const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
    /// <summary>The access control allow headers.</summary>
    private const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
    
    /// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous
    /// operation.</summary>
    ///
    /// <param name="request">          The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    ///
    /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing
    /// the asynchronous operation.</returns>
    
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        bool isCorsRequest = request.Headers.Contains(Origin);
        bool isPreflightRequest = request.Method == HttpMethod.Options;
        if(isCorsRequest)
        {
            if(isPreflightRequest)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
                string accessControlRequestMethod = request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
                if(accessControlRequestMethod != null)
                {
                    response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
                }
                string requestedHeaders = string.Join(", ", request.Headers.GetValues(AccessControlRequestHeaders));
                if(!string.IsNullOrEmpty(requestedHeaders))
                {
                    response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
                }
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                // tcs.SetResult(response);
                tcs.SetResult(new HttpResponseMessage(HttpStatusCode.OK));
                return tcs.Task;
            }
            return base.SendAsync(request, cancellationToken).ContinueWith(t =>
            {
                HttpResponseMessage resp = t.Result;
                // resp.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
                return resp;
            });
        }
        return base.SendAsync(request, cancellationToken);
    }
}
}