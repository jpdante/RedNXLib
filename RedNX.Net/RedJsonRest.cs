using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedNX.Net {
    public class RedJsonRest {

        public string BaseUri { get; set; }
        public string Accept { get; set; } = null;
        public bool AllowAutoRedirect { get; set; } = true;
        public bool AllowReadStreamBuffering { get; set; } = false;
        public bool AllowWriteStreamBuffering { get; set; } = true;
        public DecompressionMethods AutomaticDecompression { get; set; } = DecompressionMethods.All;
        public X509CertificateCollection ClientCertificates { get; set; } = new X509CertificateCollection();
        public string Connection { get; set; } = null;
        public string ConnectionGroupName { get; set; } = null;
        public int ContinueTimeout { get; set; } = 350;
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();
        public ICredentials Credentials { get; set; } = null;
        public DateTime Date { get; set; } = DateTime.MinValue;
        public string Expect { get; set; } = null;
        public WebHeaderCollection Headers { get; set; } = new WebHeaderCollection();
        public DateTime IfModifiedSince { get; set; } = DateTime.MinValue;
        public bool KeepAlive { get; set; } = true;
        public int MaximumAutomaticRedirections { get; set; } = 50;
        public int MaximumResponseHeadersLength { get; set; } = 64;
        public string MediaType { get; set; } = null;
        public bool Pipelined { get; set; } = true;
        public bool PreAuthenticate { get; set; } = false;
        public Version ProtocolVersion { get; set; } = HttpVersion.Version11;
        public IWebProxy Proxy { get; set; } = null;
        public int ReadWriteTimeout { get; set; } = 300000;
        public string Referer { get; set; } = null;
        public bool SendChunked { get; set; } = false;
        public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; } = null;
        public int Timeout { get; set; } = 100000;
        public string TransferEncoding { get; set; } = null;
        public bool UnsafeAuthenticatedConnectionSharing { get; set; } = true;
        public bool UseDefaultCredentials { get; set; } = false;
        public string UserAgent { get; set; } = null;
        public AuthenticationLevel AuthenticationLevel { get; set; } = AuthenticationLevel.MutualAuthRequested;
        public RequestCachePolicy CachePolicy { get; set; } = new RequestCachePolicy(RequestCacheLevel.BypassCache);
        public TokenImpersonationLevel ImpersonationLevel { get; set; } = TokenImpersonationLevel.Delegation;

        public RedJsonRest(string baseUrl) {
            BaseUri = baseUrl;
        }

        #region GET

        public Task<T> Get<T>(string path) => Custom<T>(path, "GET");

        public Task<JsonDocument> Get(string path) => Custom(path, "GET");

        #endregion

        #region HEAD

        public Task<T> Head<T>(string path) => Custom<T>(path, "HEAD");

        public Task<JsonDocument> Head(string path) => Custom(path, "HEAD");

        #endregion

        #region POST

        public Task<T> Post<T>(string path, object obj) => Custom<T>(path, obj, "POST");

        public Task<JsonDocument> Post(string path, object obj) => Custom(path, obj, "POST");

        public Task<T> Post<T>(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom<T>(path, "POST", stream, contentType);

        public Task<JsonDocument> Post(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "POST", stream, contentType);

        #endregion

        #region PUT

        public Task<T> Put<T>(string path, object obj) => Custom<T>(path, obj, "PUT");

        public Task<JsonDocument> Put(string path, object obj) => Custom(path, obj, "PUT");

        public Task<T> Put<T>(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom<T>(path, "PUT", stream, contentType);

        public Task<JsonDocument> Put(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "PUT", stream, contentType);

        #endregion

        #region DELETE

        public Task<T> Delete<T>(string path, object obj) => Custom<T>(path, obj, "DELETE");

        public Task<JsonDocument> Delete(string path, object obj) => Custom(path, obj, "DELETE");

        public Task<T> Delete<T>(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom<T>(path, "DELETE", stream, contentType);

        public Task<JsonDocument> Delete(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "DELETE", stream, contentType);

        #endregion

        #region CONNECT

        public Task<T> Connect<T>(string path, object obj) => Custom<T>(path, obj, "CONNECT");

        public Task<JsonDocument> Connect(string path, object obj) => Custom(path, obj, "CONNECT");

        public Task<T> Connect<T>(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom<T>(path, "CONNECT", stream, contentType);

        public Task<JsonDocument> Connect(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "CONNECT", stream, contentType);

        #endregion

        #region OPTIONS

        public Task<T> Options<T>(string path, object obj) => Custom<T>(path, obj, "OPTIONS");

        public Task<JsonDocument> Options(string path, object obj) => Custom(path, obj, "OPTIONS");

        public Task<T> Options<T>(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom<T>(path, "OPTIONS", stream, contentType);

        public Task<JsonDocument> Options(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "OPTIONS", stream, contentType);

        #endregion


        #region TRACE

        public Task<T> Trace<T>(string path, object obj) => Custom<T>(path, obj, "TRACE");

        public Task<JsonDocument> Trace(string path, object obj) => Custom(path, obj, "TRACE");

        public Task<T> Trace<T>(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom<T>(path, "TRACE", stream, contentType);

        public Task<JsonDocument> Trace(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "TRACE", stream, contentType);

        #endregion


        #region PATCH

        public Task<T> Patch<T>(string path, object obj) => Custom<T>(path, obj, "PATCH");

        public Task<JsonDocument> Patch(string path, object obj) => Custom(path, obj, "PATCH");

        public Task<T> Patch<T>(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom<T>(path, "PATCH", stream, contentType);

        public Task<JsonDocument> Patch(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "PATCH", stream, contentType);

        #endregion

        #region CUSTOM

        public async Task<T> Custom<T>(string path, object obj, string method, string contentType = "application/json") {
            var uri = new Uri($"{BaseUri}/{path}");
            var responseStream = await RequestStream(uri, method, async requestStream => {
                await JsonSerializer.SerializeAsync(requestStream, obj);
            }, contentType);
            return await JsonSerializer.DeserializeAsync<T>(responseStream);
        }

        public async Task<JsonDocument> Custom(string path, object obj, string method, string contentType = "application/json") {
            var uri = new Uri($"{BaseUri}/{path}");
            var responseStream = await RequestStream(uri, method, async requestStream => {
                await JsonSerializer.SerializeAsync(requestStream, obj);
            }, contentType);
            return await JsonDocument.ParseAsync(responseStream);
        }

        public async Task<T> Custom<T>(string path, string method, Stream stream = null, string contentType = "application/octet-stream") {
            var uri = new Uri($"{BaseUri}/{path}");
            var responseStream = await RequestStream(uri, method, stream == null ? null : async requestStream => {
                await stream.CopyToAsync(requestStream);
            }, contentType);
            return await JsonSerializer.DeserializeAsync<T>(responseStream);
        }

        public async Task<JsonDocument> Custom(string path, string method, Stream stream = null, string contentType = "application/octet-stream") {
            var uri = new Uri($"{BaseUri}/{path}");
            var responseStream = await RequestStream(uri, method, stream == null ? null : async requestStream => {
                await stream.CopyToAsync(requestStream);
            }, contentType);
            return await JsonDocument.ParseAsync(responseStream);
        }

        #endregion

        /*private async Task<WebResponse> Request(Uri uri, string method, Func<Stream, Task> onRequest = null) {
            var request = WebRequest.CreateHttp(uri);
            request.Method = method;
            if (onRequest != null) {
                var requestStream = await request.GetRequestStreamAsync();
                await onRequest.Invoke(requestStream);
            }
            return await request.GetResponseAsync();
        }*/

        private async Task<Stream> RequestStream(Uri uri, string method, Func<Stream, Task> onRequest = null, string contentType = null) {
            var request = WebRequest.CreateHttp(uri);
            request.Method = method;
            request.Accept = Accept;
            request.AllowAutoRedirect = AllowAutoRedirect;
            request.AllowReadStreamBuffering = AllowReadStreamBuffering;
            request.AllowWriteStreamBuffering = AllowWriteStreamBuffering;
            request.AutomaticDecompression = AutomaticDecompression;
            request.ClientCertificates = ClientCertificates;
            request.Connection = Connection;
            request.ConnectionGroupName = ConnectionGroupName;
            request.ContentType = contentType;
            request.ContinueTimeout = ContinueTimeout;
            request.CookieContainer = CookieContainer;
            request.Credentials = Credentials;
            request.Date = Date;
            request.Expect = Expect;
            request.Headers = Headers;
            request.IfModifiedSince = IfModifiedSince;
            request.KeepAlive = KeepAlive;
            request.MaximumAutomaticRedirections = MaximumAutomaticRedirections;
            request.MaximumResponseHeadersLength = MaximumResponseHeadersLength;
            request.MediaType = MediaType;
            request.Pipelined = Pipelined;
            request.PreAuthenticate = PreAuthenticate;
            request.ProtocolVersion = ProtocolVersion;
            request.Proxy = Proxy;
            request.ReadWriteTimeout = ReadWriteTimeout;
            request.Referer = Referer;
            request.SendChunked = SendChunked;
            request.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
            request.Timeout = Timeout;
            request.TransferEncoding = TransferEncoding;
            request.UnsafeAuthenticatedConnectionSharing = UnsafeAuthenticatedConnectionSharing;
            request.UseDefaultCredentials = UseDefaultCredentials;
            request.UserAgent = UserAgent;
            request.AuthenticationLevel = AuthenticationLevel;
            request.CachePolicy = CachePolicy;
            request.ImpersonationLevel = ImpersonationLevel;

            if (onRequest != null) {
                var requestStream = await request.GetRequestStreamAsync();
                await onRequest.Invoke(requestStream);
            }
            var response = await request.GetResponseAsync();
            var responseStream = response.GetResponseStream();
            if (responseStream == null) throw new WebException("No response.", WebExceptionStatus.ConnectionClosed);
            return responseStream;
        }

    }
}
