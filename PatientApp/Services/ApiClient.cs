using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PatientApp.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<T?> GetAsync<T>(string uri)
        => _httpClient.GetFromJsonAsync<T>(uri);

    public async Task<T?> PostAsync<T>(string uri, object? data = null)
    {
        HttpResponseMessage response = data is null
            ? await _httpClient.PostAsync(uri, null)
            : await _httpClient.PostAsJsonAsync(uri, data);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task PostAsync(string uri, object? data = null)
    {
        HttpResponseMessage response = data is null
            ? await _httpClient.PostAsync(uri, null)
            : await _httpClient.PostAsJsonAsync(uri, data);

        response.EnsureSuccessStatusCode();
    }
}
