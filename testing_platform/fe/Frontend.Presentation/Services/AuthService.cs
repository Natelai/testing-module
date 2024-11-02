using Contracts.APICommunication;
using Contracts.Internal;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Frontend.Presentation.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, ApiSettings apiSettings)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _httpClient.BaseAddress = new Uri(apiSettings.BaseUrl);
    }

    public async Task<bool> Login(string username, string password)
    {
        var loginModel = new { Username = username, Password = password };
        var content = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/login", content);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
            return true;
        }
        return false;
    }

    public async Task<bool> SignUp(string username, string password, string email)
    {
        var signUpModel = new { Username = username, Password = password, Email = email };
        var content = new StringContent(JsonSerializer.Serialize(signUpModel), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/signup", content);
        return response.IsSuccessStatusCode;
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<string> GetToken()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
    }
}