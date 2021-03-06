﻿namespace GMaster.Camera
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using LumixResponces;
    using Windows.Web.Http;
    using Windows.Web.Http.Filters;
    using Windows.Web.Http.Headers;

    public class Http
    {
        private readonly HttpClient camcgi;
        private readonly Uri baseUri;

        public Http(Uri baseUrl)
        {
            this.baseUri = baseUrl;

            var rootFilter = new HttpBaseProtocolFilter();
            rootFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            rootFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;

            camcgi = new HttpClient(rootFilter);
            camcgi.DefaultRequestHeaders.Accept.Clear();
            camcgi.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/xml"));
        }

        public static TResponse ReadResponse<TResponse>(string str)
        {
            var serializer = new XmlSerializer(typeof(TResponse));
            return (TResponse)serializer.Deserialize(new StringReader(str));
        }

        public async Task<TResponse> Get<TResponse>(string path)
            where TResponse : BaseRequestResult
        {
            var uri = new Uri(baseUri, path);
            using (var response = await camcgi.GetAsync(uri))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new LumixException("Request failed: " + path);
                }

                var product = await ReadResponse<TResponse>(response);
                if (product.Result != "ok")
                {
                    throw new LumixException(
                        $"Not ok result\r\nRequest: {path}\r\n{product.Result}");
                }

                return product;
            }
        }

        public async Task<TResponse> Get<TResponse>(Dictionary<string, string> parameters)
            where TResponse : BaseRequestResult
        {
            var uri = new Uri(baseUri, "?" + string.Join("&", parameters.Select(p => p.Key + "=" + p.Value)));
            using (var response = await camcgi.GetAsync(uri))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new LumixException("Request failed: ");
                }

                var product = await ReadResponse<TResponse>(response);
                if (product.Result != "ok")
                {
                    throw new LumixException(
                        $"Not ok result\r\nRequest: \r\n{product.Result}");
                }

                return product;
            }
        }

        public async Task<string> GetString(string path)
        {
            var uri = new Uri(baseUri, path);
            using (var response = await camcgi.GetAsync(uri))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new LumixException("Request failed: " + path);
                }

                return await response.Content.ReadAsStringAsync();
            }
        }

        private static async Task<TResponse> ReadResponse<TResponse>(HttpResponseMessage response)
        {
            using (var content = response.Content)
            using (var str = await content.ReadAsInputStreamAsync())
            {
                var serializer = new XmlSerializer(typeof(TResponse));
                return (TResponse)serializer.Deserialize(str.AsStreamForRead());
            }
        }
    }
}
