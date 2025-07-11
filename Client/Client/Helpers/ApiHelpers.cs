using Client.Const;
using Client.LocalStorage;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Client.Helpers
{
    public class ApiResponse
    {
        public string ResponseBody { get; set; }
        public int StatusCode { get; set; }
    }

    public class ApiRequestGet
    {
        public string ApiUri { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public bool IsUseToken { get; set; }

        public ApiRequestGet(string apiUri, bool isUseToken = true, Dictionary<string, string> headers = null)
        {
            ApiUri = apiUri;
            Headers = headers;
            IsUseToken = isUseToken;
        }
    }

    public class ApiRequest
    {
        public string ApiUri { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
        public bool IsUseToken { get; set; }

        public ApiRequest(string apiUri, string body, bool isUseToken = true, Dictionary<string, string> headers = null)
        {
            ApiUri = apiUri;
            Body = body;
            Headers = headers;
            IsUseToken = isUseToken;
        }
    }

    public static class ApiHelpers
    {
        private static readonly HttpClient client = new HttpClient();

        private static void AddTokenHeader(HttpRequestMessage httpRequest, bool isUseToken)
        {
            if (isUseToken)
            {
                httpRequest.Headers.Add("Authorization", "Bearer " + UserStore.Token); 
            }
        }

        // GET
        public static async Task<ApiResponse> GetAsync(ApiRequestGet request)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, ConfigConst.BaseApiUrl + request.ApiUri);
            AddTokenHeader(httpRequest, request.IsUseToken);
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }
            HttpResponseMessage response = await client.SendAsync(httpRequest);
            string responseBody = await response.Content.ReadAsStringAsync();

            return new ApiResponse
            {
                ResponseBody = responseBody,
                StatusCode = (int)response.StatusCode
            };
        }

        // POST
        public static async Task<ApiResponse> PostAsync(ApiRequest request)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, ConfigConst.BaseApiUrl + request.ApiUri)
            {
                Content = new StringContent(request.Body, Encoding.UTF8, "application/json")
            };

            AddTokenHeader(httpRequest, request.IsUseToken);
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage response = await client.SendAsync(httpRequest);
            string responseBody = await response.Content.ReadAsStringAsync();

            return new ApiResponse
            {
                ResponseBody = responseBody,
                StatusCode = (int)response.StatusCode
            };
        }

        // PUT
        public static async Task<ApiResponse> PutAsync(ApiRequest request)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Put, ConfigConst.BaseApiUrl + request.ApiUri)
            {
                Content = new StringContent(request.Body, Encoding.UTF8, "application/json")
            };
            AddTokenHeader(httpRequest, request.IsUseToken);
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage response = await client.SendAsync(httpRequest);
            string responseBody = await response.Content.ReadAsStringAsync();

            return new ApiResponse
            {
                ResponseBody = responseBody,
                StatusCode = (int)response.StatusCode
            };
        }

        // DELETE
        public static async Task<ApiResponse> DeleteAsync(ApiRequestGet request)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Delete, ConfigConst.BaseApiUrl + request.ApiUri);
            AddTokenHeader(httpRequest, request.IsUseToken);
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage response = await client.SendAsync(httpRequest);
            string responseBody = await response.Content.ReadAsStringAsync();

            return new ApiResponse
            {
                ResponseBody = responseBody,
                StatusCode = (int)response.StatusCode
            };
        }


        // PATCH
        public static async Task<ApiResponse> PatchAsync(ApiRequest request)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Patch, ConfigConst.BaseApiUrl + request.ApiUri)
            {
                Content = new StringContent(request.Body, Encoding.UTF8, "application/json")
            };
            AddTokenHeader(httpRequest, request.IsUseToken);
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage response = await client.SendAsync(httpRequest);
            string responseBody = await response.Content.ReadAsStringAsync();

            return new ApiResponse
            {
                ResponseBody = responseBody,
                StatusCode = (int)response.StatusCode
            };
        }


        public static async Task<ApiResponse> PostFileAsync(string apiUri, List<(string fileName, byte[] content)> files, bool isUseToken = true)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, ConfigConst.BaseApiUrl + apiUri);
            using var content = new MultipartFormDataContent();

            foreach (var (fileName, fileBytes) in files)
            {
                var byteContent = new ByteArrayContent(fileBytes);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                content.Add(byteContent, "files", fileName);
            }

            AddTokenHeader(request, isUseToken);

            request.Content = content;

            var response = await client.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();

            return new ApiResponse
            {
                ResponseBody = responseBody,
                StatusCode = (int)response.StatusCode
            };
        }

    }
}
