using System.Text.Json;

namespace Presentacion.Services
{
    public class HttpClientService
    {
        private readonly HttpClient _httpClient;

        public HttpClientService(HttpClient httpClient, IConfiguration configuration)
        {
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var fullUrl = new Uri(_httpClient.BaseAddress, endpoint); // Construir la URL completa

                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, jsonContent);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<TResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error during POST request", ex);
            }
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, jsonContent);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<TResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error during PUT request", ex);
            }
        }

        public async Task<TResponse> DeleteAsync<TResponse>(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<TResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error during PUT request", ex);
            }
        }

    }
}
