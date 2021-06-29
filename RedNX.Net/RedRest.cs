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
    public class RedRest {

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

        public RedRest(string baseUrl) {
            BaseUri = baseUrl;
        }

        #region GET

        public Task<RestResponse> Get(string path) => Custom(path, "GET");

        #endregion

        #region HEAD

        public Task<RestResponse> Head(string path) => Custom(path, "HEAD");

        #endregion

        #region POST

        public Task<RestResponse> Post(string path, Func<Stream, Task> onRequest, string contentType = "application/octet-stream") => Custom(path, "POST", onRequest, contentType);

        public Task<RestResponse> Post(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "POST", stream, contentType);

        #endregion

        #region PUT

        public Task<RestResponse> Put(string path, Func<Stream, Task> onRequest, string contentType = "application/octet-stream") => Custom(path, "PUT", onRequest, contentType);

        public Task<RestResponse> Put(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "PUT", stream, contentType);

        #endregion

        #region DELETE

        public Task<RestResponse> Delete(string path, Func<Stream, Task> onRequest, string contentType = "application/octet-stream") => Custom(path, "DELETE", onRequest, contentType);

        public Task<RestResponse> Delete(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "DELETE", stream, contentType);

        #endregion

        #region CONNECT

        public Task<RestResponse> Connect(string path, Func<Stream, Task> onRequest, string contentType = "application/octet-stream") => Custom(path, "CONNECT", onRequest, contentType);

        public Task<RestResponse> Connect(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "CONNECT", stream, contentType);

        #endregion

        #region OPTIONS

        public Task<RestResponse> Options(string path, Func<Stream, Task> onRequest, string contentType = "application/octet-stream") => Custom(path, "OPTIONS", onRequest, contentType);

        public Task<RestResponse> Options(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "OPTIONS", stream, contentType);

        #endregion

        #region TRACE

        public Task<RestResponse> Trace(string path, Func<Stream, Task> onRequest, string contentType = "application/octet-stream") => Custom(path, "TRACE", onRequest, contentType);

        public Task<RestResponse> Trace(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "TRACE", stream, contentType);

        #endregion

        #region PATCH

        public Task<RestResponse> Patch(string path, Func<Stream, Task> onRequest, string contentType = "application/octet-stream") => Custom(path, "PATCH", onRequest, contentType);

        public Task<RestResponse> Patch(string path, Stream stream = null, string contentType = "application/octet-stream") => Custom(path, "PATCH", stream, contentType);

        #endregion

        #region CUSTOM

        public async Task<RestResponse> Custom(string path, string method, Func<Stream, Task> onRequest, string contentType = "application/octet-stream") {
            var uri = new Uri($"{BaseUri}/{path}");
            var response = await Request(uri, method, onRequest, contentType);
            return response;
        }

        public async Task<RestResponse> Custom(string path, string method, Stream stream = null, string contentType = "application/octet-stream") {
            var uri = new Uri($"{BaseUri}/{path}");
            var response = await Request(uri, method, stream == null ? null : async requestStream => {
                await stream.CopyToAsync(requestStream);
            }, contentType);
            return response;
        }

        #endregion

        private async Task<RestResponse> Request(Uri uri, string method, Func<Stream, Task> onRequest = null, string contentType = null) {
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
            return new RestResponse((HttpWebResponse) response, responseStream);
        }

        public class RestResponse : IDisposable {

            private readonly HttpWebResponse _response;

            public Stream Stream { get; }

            public WebHeaderCollection Header => _response.Headers;
            public string CharacterSet => _response.CharacterSet;
            public string ContentEncoding => _response.ContentEncoding;
            public long ContentLength => _response.ContentLength;
            public string ContentType => _response.ContentType;
            public CookieCollection Cookies => _response.Cookies;
            public bool IsMutuallyAuthenticated => _response.IsMutuallyAuthenticated;
            public DateTime LastModified => _response.LastModified;
            public string Method => _response.Method;
            public Version ProtocolVersion => _response.ProtocolVersion;
            public Uri ResponseUri => _response.ResponseUri;
            public string Server => _response.Server;
            public HttpStatusCode StatusCode => _response.StatusCode;
            public string StatusDescription => _response.StatusDescription;
            public bool SupportsHeaders => _response.SupportsHeaders;
            public bool IsFromCache => _response.IsFromCache;

            internal RestResponse(HttpWebResponse response, Stream stream) {
                _response = response;
                Stream = stream;
            }

            public void Dispose() {
                Stream?.Dispose();
                _response?.Dispose();
            }
        }
    }
}
