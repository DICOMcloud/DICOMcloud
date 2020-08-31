// using System.Net;
// using System.Net.Http;
// using System.Threading;
// using System.Threading.Tasks;

// public class PreflightRequestsHandler : DelegatingHandler
// {
//     public PreflightRequestsHandler()
//     { }

//     public PreflightRequestsHandler (string origins, string headers, string methods)
//     { 
//         _origins = origins;
//         _headers = headers;
//         _methods = methods;
//     }

//     private string _origins = "*";
//     private string _headers = "*"; //"Origin, X-Requested-With, Content-Type, Accept, Authorization";
//     private string _methods = "*"; //"GET,HEAD,OPTIONS,POST,PUT";
//     protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//     {
//         if (request.Headers.Contains("Origin") && request.Method.Method.Equals("OPTIONS"))
//         {
//             var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
//             // Define and add values to variables: origins, headers, methods (can be global)               
//             response.Headers.Add("Access-Control-Allow-Origin", _origins);
//             response.Headers.Add("Access-Control-Allow-Headers", _headers);
//             response.Headers.Add("Access-Control-Allow-Methods", _methods);
            
//             var tsc = new TaskCompletionSource<HttpResponseMessage>();
//             tsc.SetResult(response);
//             return tsc.Task;
//         }
//         return base.SendAsync(request, cancellationToken);
//     }

// }