using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ServerlessPDFConversionDemo
{
    public class FileService
    {
        private readonly AuthenticationService _authenticationService;
        private HttpClient _httpClient;

        public FileService(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        private async Task<HttpClient> CreateAuthorizedHttpClient()
        {
            if (_httpClient != null)
            {
                return _httpClient;
            }

            var token = await _authenticationService.GetAccessTokenAsync();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            return _httpClient;
        }

        public async Task<string> UploadStreamAsync(string path, Stream content, string contentType)
        {
            var httpClient = await CreateAuthorizedHttpClient();

            string tmpFileName = $"{Guid.NewGuid().ToString()}.{MimeTypes.MimeTypeMap.GetExtension(contentType)}";
            string requestUrl = $"{path}root:/{tmpFileName}:/content";
            var requestContent = new StreamContent(content);
            requestContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            var response = await httpClient.PutAsync(requestUrl, requestContent);
            if (response.IsSuccessStatusCode)
            {
                dynamic file = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                return file?.id;
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Upload file failed with status {response.StatusCode} and message {message}");
            }
        }

        public async Task<byte[]> DownloadConvertedFileAsync(string path, string fileId, string targetFormat)
        {
            var httpClient = await CreateAuthorizedHttpClient();

            var requestUrl = $"{path}{fileId}/content?format={targetFormat}";
            var response = await httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var fileContent = await response.Content.ReadAsByteArrayAsync();
                return fileContent;
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Download of converted file failed with status {response.StatusCode} and message {message}");
            }
        }

        public async Task DeleteFileAsync(string path, string fileId)
        {
            var httpClient = await CreateAuthorizedHttpClient();

            var requestUrl = $"{path}{fileId}";
            var response = await httpClient.DeleteAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Delete file failed with status {response.StatusCode} and message {message}");
            }
        }
    }
}
