using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dywham.Fabric.Providers.Web.Comms.RestClient
{
    public class DywhamRestClient : IDisposable, IDywhamRestClient
    {
        private Action<HttpRequestMessage, HttpResponseMessage, ApiRequestException> _onRequestError;
        private bool _throwOnException;


        public DywhamRestClient()
        {
            Init();
        }


        public HttpClient HttpClient { get; private set; }

        public Action<HttpRequestMessage> OnBeforeRequest { get; set; }


        public void Init(HttpMessageHandler messageHandler = null)
        {
            HttpClient = messageHandler == null ? new HttpClient() : new HttpClient(messageHandler);

            _throwOnException = true;
        }

        public void UseRequestErrorTracing(Action<HttpRequestMessage, HttpResponseMessage, ApiRequestException> onRequestError, bool throwOnException)
        {
            _onRequestError = onRequestError;
            _throwOnException = throwOnException;
        }

        public DywhamRestClient AddDefaultRequestHeaders(Dictionary<string, object> defaultRequestHeaders)
        {
            if (defaultRequestHeaders == null || !defaultRequestHeaders.Any()) return this;

            foreach (var key in defaultRequestHeaders.Keys)
            {
                HttpClient.DefaultRequestHeaders.Add(key, defaultRequestHeaders[key].ToString());
            }

            return this;
        }

        public DywhamRestResponse DoHead(DywhamRestRequestParams submitSettings)
        {
            return DoRequestAsync(HttpMethod.Head, submitSettings, CancellationToken.None).Result;
        }

        public Task<DywhamRestResponse> DoHeadAsync(string url, CancellationToken ct)
        {
            return DoRequestAsync(HttpMethod.Head, new DywhamRestRequestParams
            {
                Url = url
            }, CancellationToken.None);
        }

        public async Task<DywhamRestResponse> DoHeadAsync(Dictionary<string, object> requestHeaders,
            CancellationToken ct)
        {
            return await DoRequestAsync(HttpMethod.Head, new DywhamRestRequestParams
            {
                RequestHeaders = requestHeaders
            }, CancellationToken.None);
        }

        public async Task<DywhamRestResponse> DoHeadAsync(DywhamRestRequestParams submitSettings, CancellationToken ct)
        {
            return await DoRequestAsync(HttpMethod.Head, submitSettings, ct);
        }

        public async Task<DywhamRestResponse> DoDeleteAsync(string url, CancellationToken ct)
        {
            return await DoDeleteAsync(new SubmitDywhamHttpRequestParams {Url = url}, ct);
        }

        public async Task<DywhamRestResponse> DoDeleteAsync(SubmitDywhamHttpRequestParams httpRequestParams, CancellationToken ct)
        {
            return await DoRequestAsync(HttpMethod.Delete, httpRequestParams, ct);
        }

        public DywhamRestResponse DoPost(SubmitDywhamHttpRequestParams submitSettings)
        {
            return DoRequestAsync(HttpMethod.Head, submitSettings, CancellationToken.None).Result;
        }

        public DywhamHttpResponse<T> DoPost<T>(SubmitDywhamHttpRequestParams httpRequestParams)
        {
            return DoRequestAsync<T>(HttpMethod.Post, httpRequestParams, CancellationToken.None).Result;
        }

        public async Task<DywhamHttpResponse<T>> DoPostAsync<T>(SubmitDywhamHttpRequestParams httpRequestParams, CancellationToken ct)
        {
            return await DoRequestAsync<T>(HttpMethod.Post, httpRequestParams, CancellationToken.None);
        }

        public DywhamHttpResponse<T> DoPost<T>(FilePostDywhamHttpRequestParams httpRequestParams)
        {
            return DoRequestAsync<T>(HttpMethod.Post, httpRequestParams, CancellationToken.None).Result;
        }

        public async Task<DywhamHttpResponse<T>> DoPostAsync<T>(FilePostDywhamHttpRequestParams httpRequestParams, CancellationToken ct)
        {
            return await DoRequestAsync<T>(HttpMethod.Post, httpRequestParams, ct);
        }

        public async Task<DywhamRestResponse> DoPostAsync(FilePostDywhamHttpRequestParams httpRequestParams, CancellationToken ct)
        {
            return await DoRequestAsync(HttpMethod.Post, httpRequestParams, ct);
        }

        public async Task<DywhamRestResponse> DoPostAsync(SubmitDywhamHttpRequestParams httpRequestParams, CancellationToken ct)
        {
            return await DoRequestAsync(HttpMethod.Post, httpRequestParams, ct);
        }

        public async Task<DywhamRestResponse> DoPatchAsync(SubmitDywhamHttpRequestParams httpRequestParams, CancellationToken ct)
        {
            return await DoRequestAsync(new HttpMethod("PATCH"), httpRequestParams, ct);
        }

        public async Task<DywhamRestResponse> DoPutAsync(SubmitDywhamHttpRequestParams httpRequestParams, CancellationToken ct)
        {
            return await DoRequestAsync(HttpMethod.Put, httpRequestParams, ct);
        }

        public DywhamHttpResponse<T> DoGet<T>(DywhamRestRequestParams submitSettings)
        {
            return DoRequestAsync<T>(HttpMethod.Get, submitSettings, CancellationToken.None).Result;
        }

        public async Task<DywhamHttpResponse<T>> DoGetAsync<T>(string url, CancellationToken ct)
        {
            return await DoGetAsync<T>(new DywhamRestRequestParams {Url = url}, ct);
        }

        public async Task<DywhamHttpResponse<T>> DoGetAsync<T>(DywhamRestRequestParams submitSettings, CancellationToken ct)
        {
            return await DoRequestAsync<T>(HttpMethod.Get, submitSettings, ct);
        }

        public async Task<DywhamHttpResponse<T>> DoGetAsync<T>(Dictionary<string, object> requestHeaders, CancellationToken ct)
        {
            return await DoRequestAsync<T>(HttpMethod.Get, new DywhamRestRequestParams
            {
                RequestHeaders = requestHeaders
            }, ct);
        }

        public async Task<DywhamHttpResponse<T>> DoRequestAsync<T>(HttpMethod httpMethod, string url, CancellationToken ct)
        {
            return await DoRequestAsync<T>(httpMethod, new DywhamRestRequestParams {Url = url}, ct);
        }

        public async Task<DywhamHttpResponse<T>> DoRequestAsync<T>(HttpMethod httpMethod, string url, object payload, CancellationToken ct)
        {
            return await DoRequestAsync<T>(httpMethod, new SubmitDywhamHttpRequestParams {Url = url, Data = payload}, ct);
        }

        public Task<DywhamRestFileResponse> DownloadFileAsync(string url, CancellationToken ct)
        {
            return DownloadFileAsync(new DywhamRestRequestParams { Url = url}, ct);
        }

        public async Task<DywhamRestFileResponse> DownloadFileAsync(DywhamRestRequestParams httpRequestParams, CancellationToken ct)
        {
            var url = httpRequestParams.QuerystringParams == null
                ? httpRequestParams.Url
                : ResolveRoute(httpRequestParams.Url, httpRequestParams.QuerystringParams);
            var requestMessage = new HttpRequestMessage { RequestUri = new Uri(url), Method = HttpMethod.Get };

            OnBeforeRequest?.Invoke(requestMessage);

            if (httpRequestParams is FilePostDywhamHttpRequestParams requestParams)
            {
                PrepareHttpRequestForBinaryMessage(requestMessage, requestParams);
            }
            else if (httpRequestParams is SubmitDywhamHttpRequestParams submitDywhamHttpRequestParams)
            {
                PrepareHttpRequestForTextMessage(requestMessage, submitDywhamHttpRequestParams.Data,
                    submitDywhamHttpRequestParams.RequestHeaders,
                    submitDywhamHttpRequestParams.JsonSerializerSettingsOutgoing);
            }
            else
            {
                PrepareHttpRequestForTextMessage(requestMessage, null, httpRequestParams.RequestHeaders);
            }

            httpRequestParams.OnBeforeRequest?.Invoke(requestMessage);

            var onRequestErrorExecuted = false;
            var responseMessage = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
            var message = string.Empty;
            var headers = responseMessage.Headers.ToDictionary(x => x.Key, x => x.Value);

            if (!responseMessage.IsSuccessStatusCode)
            {
                message = await ProcessErrorResponse(requestMessage, responseMessage);

                onRequestErrorExecuted = true;
            }

            httpRequestParams.OnAfterRequest?.Invoke(responseMessage, message);

            return new DywhamRestFileResponse(headers, onRequestErrorExecuted, responseMessage.StatusCode,
                await responseMessage.Content.ReadAsStreamAsync(ct));
        }

        public async Task<DywhamRestResponse> DoRequestAsync(HttpMethod httpMethod, DywhamRestRequestParams httpRequestParams, CancellationToken ct)
        {
            var url = httpRequestParams.QuerystringParams == null
                ? httpRequestParams.Url
                : ResolveRoute(httpRequestParams.Url, httpRequestParams.QuerystringParams);
            var requestMessage = new HttpRequestMessage {RequestUri = new Uri(url), Method = httpMethod};

            OnBeforeRequest?.Invoke(requestMessage);

            if (httpRequestParams is FilePostDywhamHttpRequestParams requestParams)
            {
                PrepareHttpRequestForBinaryMessage(requestMessage, requestParams);
            }
            else if (httpRequestParams is SubmitDywhamHttpRequestParams submitDywhamHttpRequestParams)
            {
                PrepareHttpRequestForTextMessage(requestMessage, submitDywhamHttpRequestParams.Data,
                    submitDywhamHttpRequestParams.RequestHeaders,
                    submitDywhamHttpRequestParams.JsonSerializerSettingsOutgoing);
            }
            else
            {
                PrepareHttpRequestForTextMessage(requestMessage, null, httpRequestParams.RequestHeaders);
            }

            httpRequestParams.OnBeforeRequest?.Invoke(requestMessage);

            var onRequestErrorExecuted = false;

            using (var responseMessage = await HttpClient.SendAsync(requestMessage, ct))
            {
                var message = string.Empty;
                var headers = responseMessage.Headers.ToDictionary(x => x.Key, x => x.Value);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    message = await ProcessErrorResponse(requestMessage, responseMessage);

                    onRequestErrorExecuted = true;
                }

                httpRequestParams.OnAfterRequest?.Invoke(responseMessage, message);

                return new DywhamRestResponse(headers, onRequestErrorExecuted, responseMessage.StatusCode);
            }
        }

        public async Task<DywhamHttpResponse<T>> DoRequestAsync<T>(HttpMethod httpMethod,  DywhamRestRequestParams httpRequestParams, CancellationToken ct)
        {
            var url = httpRequestParams.QuerystringParams == null
                ? httpRequestParams.Url
                : ResolveRoute(httpRequestParams.Url, httpRequestParams.QuerystringParams);
            var requestMessage = new HttpRequestMessage {RequestUri = new Uri(url), Method = httpMethod};

            OnBeforeRequest?.Invoke(requestMessage);

            switch (httpRequestParams)
            {
                case FilePostDywhamHttpRequestParams requestParams:

                    PrepareHttpRequestForBinaryMessage(requestMessage, requestParams);

                    break;

                case SubmitDywhamHttpRequestParams submitDywhamHttpRequestParams:

                    PrepareHttpRequestForTextMessage(requestMessage, submitDywhamHttpRequestParams.Data,
                        submitDywhamHttpRequestParams.RequestHeaders,
                        submitDywhamHttpRequestParams.JsonSerializerSettingsOutgoing);

                    break;

                default:

                    PrepareHttpRequestForTextMessage(requestMessage, null, httpRequestParams.RequestHeaders);

                    break;
            }

            httpRequestParams.OnBeforeRequest?.Invoke(requestMessage);

            var onRequestErrorExecuted = false;

            using (var responseMessage = await HttpClient.SendAsync(requestMessage, ct))
            {
                var message = string.Empty;
                var headers = responseMessage.Headers.ToDictionary(x => x.Key, x => x.Value);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    message = await ProcessErrorResponse(requestMessage, responseMessage);

                    onRequestErrorExecuted = true;
                }

                if (!responseMessage.IsSuccessStatusCode ||
                    responseMessage.StatusCode == HttpStatusCode.NoContent || httpMethod == HttpMethod.Delete ||
                    httpMethod == HttpMethod.Head)
                {
                    httpRequestParams.OnAfterRequest?.Invoke(responseMessage, message);

                    return new DywhamHttpResponse<T>(headers, onRequestErrorExecuted, responseMessage.StatusCode);
                }

                message = await responseMessage.Content.ReadAsStringAsync(ct);

                if (typeof(T) == typeof(string) || typeof(T).IsPrimitive)
                {
                    return new DywhamHttpResponse<T>((T) Convert.ChangeType(message, typeof(T)), headers, false,
                        responseMessage.StatusCode);
                }

                return new DywhamHttpResponse<T>(JsonConvert.DeserializeObject<T>(message, httpRequestParams.JsonSerializerSettingsIncoming),
                    headers, false, responseMessage.StatusCode);
            }
        }

        private async Task<string> ProcessErrorResponse(HttpRequestMessage httpRequestMessage, HttpResponseMessage httpResponseMessage)
        {
            var errorMessage = string.Empty;

            try
            {
                errorMessage = await httpResponseMessage.Content.ReadAsStringAsync();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            { }

            var apiRequestException = new ApiRequestException(errorMessage,
                // ReSharper disable once PossibleNullReferenceException
                httpRequestMessage.RequestUri.AbsoluteUri, httpResponseMessage.StatusCode);

            _onRequestError?.Invoke(httpRequestMessage, httpResponseMessage, apiRequestException);

            if (_throwOnException)
            {
                throw apiRequestException;
            }

            return errorMessage;
        }

        private static void PrepareHttpRequestForTextMessage(HttpRequestMessage httpRequestMessage, object data, Dictionary<string, object> requestHeaders, JsonSerializerSettings serializerSettings = null)
        {
            var serializedData = string.Empty;

            if (data != null)
            {
                serializedData = serializerSettings == null
                    ? JsonConvert.SerializeObject(data)
                    : JsonConvert.SerializeObject(data, serializerSettings);
            }

            if (!string.IsNullOrWhiteSpace(serializedData))
            {
                httpRequestMessage.Content = new StringContent(serializedData, Encoding.UTF8, "application/json");
            }

            if (requestHeaders == null || !requestHeaders.Any()) return;

            foreach (var key in requestHeaders.Keys)
            {
                httpRequestMessage.Headers.TryAddWithoutValidation(key, requestHeaders[key].ToString());
            }
        }

        private static void PrepareHttpRequestForBinaryMessage(HttpRequestMessage httpRequestMessage, FilePostDywhamHttpRequestParams requestParams)
        {
            var stream = requestParams.Stream;

            if (stream == null)
            {
                if (string.IsNullOrEmpty(requestParams.FileName))
                {
                    requestParams.FileName = Guid.NewGuid().ToString();
                }

                if (!string.IsNullOrEmpty(requestParams.FilePath) && File.Exists(requestParams.FilePath))
                {
                    stream = new FileStream(requestParams.FilePath, FileMode.Open);
                }
                else
                {
                    stream = new MemoryStream(requestParams.FileContent);
                }
            }

            var fileContent = new StreamContent(stream);

            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file\"",
                FileName = "\"" + Path.GetFileName(requestParams.FileName) + "\"",
            };

            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.Content = new MultipartFormDataContent {fileContent};
        }

        private static string ResolveRoute(string route, object payload)
        {
            var querystringParams = payload.GetType().GetRuntimeProperties()
                .Where(x => x.GetValue(payload) != null
                            // ReSharper disable once PossibleNullReferenceException
                            && !string.IsNullOrWhiteSpace(x.GetValue(payload).ToString()))
                .ToDictionary(x => x, x => x.GetValue(payload));
            var uriBuilder = new StringBuilder(new UriBuilder(new Uri($"{route}").AbsoluteUri).ToString());

            if (!querystringParams.Any())
            {
                return uriBuilder.ToString();
            }

            uriBuilder.Append("?");

            foreach (var param in querystringParams.Where(x => x.Value != null))
            {
                if (typeof(IList).IsAssignableFrom(param.Key.PropertyType))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    foreach (var arrayParams in (IList) param.Value)
                    {
                        uriBuilder.Append($"{param.Key.Name}={arrayParams}&");
                    }

                    continue;
                }

                if (typeof(DateTime?).IsAssignableFrom(param.Key.PropertyType))
                {
                    var date = param.Value as DateTime?;

                    if (!date.HasValue) continue;

                    var dateTime = date.Value.ToString("s", CultureInfo.InvariantCulture);

                    uriBuilder.Append($"{param.Key.Name}={dateTime}&");

                    continue;
                }

                if (typeof(DateTime).IsAssignableFrom(param.Key.PropertyType))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var date = (DateTime) param.Value;
                    var dateTime = date.ToString("s", CultureInfo.InvariantCulture);

                    uriBuilder.Append($"{param.Key.Name}={dateTime}&");

                    continue;
                }

                uriBuilder.Append($"{param.Key.Name}={param.Value}&");
            }

            uriBuilder = new StringBuilder(uriBuilder.ToString()[..(uriBuilder.Length - 1)]);

            return uriBuilder.ToString();
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}