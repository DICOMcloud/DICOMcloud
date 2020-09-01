namespace DICOMcloud.Wado
{
    #region Usings

    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Routing;

    #endregion

    /// <summary>
    ///     The WadoExtensions.
    /// </summary>
    public static class WadoExtensions
    {

        public static NameValueCollection ParseQuery(this Uri uri)
        {
            // Todo: This needs to be checked.
            var result = new NameValueCollection();

            var parsedQuery = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

            foreach (var queryItem in parsedQuery)
            {
                result.Add(queryItem.Key, queryItem.Value.ToString());
            }

            return result;
        }

        public static HttpRequestMessage ToHttpRequestMessage(this HttpRequest req)
        => new HttpRequestMessage()
            .SetMethod(req)
            .SetAbsoluteUri(req)
            .SetHeaders(req)
            .SetContent(req)
            .SetContentType(req);

        private static HttpRequestMessage SetAbsoluteUri(this HttpRequestMessage msg, HttpRequest req)
            => msg.Set(m => m.RequestUri = new UriBuilder
            {
                Scheme = req.Scheme,
                Host = req.Host.Host,
                Port = req.Host.Port.Value,
                Path = req.PathBase.Add(req.Path),
                Query = req.QueryString.ToString()
            }.Uri);

        private static HttpRequestMessage SetMethod(this HttpRequestMessage msg, HttpRequest req)
            => msg.Set(m => m.Method = new HttpMethod(req.Method));

        private static HttpRequestMessage SetHeaders(this HttpRequestMessage msg, HttpRequest req)
            => req.Headers.Aggregate(msg, (acc, h) => acc.Set(m => m.Headers.TryAddWithoutValidation(h.Key, h.Value.AsEnumerable())));

        private static HttpRequestMessage SetContent(this HttpRequestMessage msg, HttpRequest req)
            => msg.Set(m => m.Content = new StreamContent(req.Body));

        private static HttpRequestMessage SetContentType(this HttpRequestMessage msg, HttpRequest req)
            => msg.Set(m => m.Content.Headers.Add("Content-Type", req.ContentType), applyIf: req.Headers.ContainsKey("Content-Type"));

        private static HttpRequestMessage Set(this HttpRequestMessage msg, Action<HttpRequestMessage> config, bool applyIf = true)
        {
            if (applyIf)
            {
                config.Invoke(msg);
            }

            return msg;
        }
         public static string ReturnAbsolutePath(this HttpRequest request) {
             var absoluteUri = string.Concat(
                        request.Scheme,
                        "://",
                        request.Host.ToUriComponent(),
                        request.PathBase.ToUriComponent(),
                        request.Path.ToUriComponent(),
                        request.QueryString.ToUriComponent());
            return absoluteUri;
         }
    }
}