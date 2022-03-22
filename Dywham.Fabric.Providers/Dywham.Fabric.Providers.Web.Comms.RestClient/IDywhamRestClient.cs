using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dywham.Fabric.Providers.Web.Comms.RestClient
{
    public interface IDywhamRestClient : IProvider
    {
        HttpClient HttpClient { get; }

        Action<HttpRequestMessage> OnBeforeRequest { get; set; }


        void Init(HttpMessageHandler messageHandler = null);

        void UseRequestErrorTracing(Action<HttpRequestMessage, HttpResponseMessage, ApiRequestException> onRequestError, bool throwOnException);

        DywhamRestClient AddDefaultRequestHeaders(Dictionary<string, object> defaultRequestHeaders);

        DywhamRestResponse DoHead(DywhamRestRequestParams submitSettings);

        Task<DywhamRestResponse> DoHeadAsync(string url, CancellationToken ct);

        Task<DywhamRestResponse> DoHeadAsync(DywhamRestRequestParams submitSettings, CancellationToken ct);

        Task<DywhamRestResponse> DoHeadAsync(Dictionary<string, object> requestHeaders, CancellationToken ct);

        Task<DywhamRestResponse> DoDeleteAsync(SubmitDywhamHttpRequestParams httpRequestParams, CancellationToken ct);

        Task<DywhamRestResponse> DoDeleteAsync(string url, CancellationToken ct);

        DywhamHttpResponse<T> DoGet<T>(DywhamRestRequestParams submitSettings);

        Task<DywhamHttpResponse<T>> DoGetAsync<T>(string url, CancellationToken ct);

        Task<DywhamHttpResponse<T>> DoGetAsync<T>(DywhamRestRequestParams submitSettings, CancellationToken ct);

        Task<DywhamHttpResponse<T>> DoGetAsync<T>(Dictionary<string, object> requestHeaders, CancellationToken ct);

        Task<DywhamRestResponse> DoPatchAsync(SubmitDywhamHttpRequestParams httpRequestParams, CancellationToken ct);

        DywhamRestResponse DoPost(SubmitDywhamHttpRequestParams httpRequestParams);

        DywhamHttpResponse<T> DoPost<T>(SubmitDywhamHttpRequestParams httpRequestParams);

        Task<DywhamRestResponse> DoPostAsync(SubmitDywhamHttpRequestParams httpRequestParams, CancellationToken ct);

        Task<DywhamHttpResponse<T>> DoPostAsync<T>(SubmitDywhamHttpRequestParams httpRequestParams, CancellationToken ct);

        DywhamHttpResponse<T> DoPost<T>(FilePostDywhamHttpRequestParams httpRequestParams);

        Task<DywhamHttpResponse<T>> DoPostAsync<T>(FilePostDywhamHttpRequestParams httpRequestParams, CancellationToken ct);

        Task<DywhamRestResponse> DoPostAsync(FilePostDywhamHttpRequestParams httpRequestParams, CancellationToken ct);

        Task<DywhamRestResponse> DoPutAsync(SubmitDywhamHttpRequestParams httpRequestParams, CancellationToken ct);

        Task<DywhamRestResponse> DoRequestAsync(HttpMethod httpMethod, DywhamRestRequestParams httpRequestParams, CancellationToken ct);

        Task<DywhamHttpResponse<T>> DoRequestAsync<T>(HttpMethod httpMethod, string url, CancellationToken ct);

        Task<DywhamHttpResponse<T>> DoRequestAsync<T>(HttpMethod httpMethod, DywhamRestRequestParams httpRequestParams, CancellationToken ct);

        Task<DywhamHttpResponse<T>> DoRequestAsync<T>(HttpMethod httpMethod, string url, object payload, CancellationToken ct);

        Task<DywhamRestFileResponse> DownloadFileAsync(string url, CancellationToken ct);

        Task<DywhamRestFileResponse> DownloadFileAsync(DywhamRestRequestParams httpRequestParams, CancellationToken ct);
    }
}