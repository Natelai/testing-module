using System.Net.Http.Headers;
using Contracts.APICommunication;
using Contracts.Internal;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Frontend.Presentation.Services;

public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public UserService(HttpClient httpClient, AuthService authService, ApiSettings apiSettings)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<ShortProfileUserDto> GetShortUserProfileAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await _authService.GetToken());

        var response = await _httpClient.GetAsync("api/user/short-profile");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ShortProfileUserDto>();
        }

        throw new HttpRequestException("Failed to load user profile");
    }

    public async Task<FullProfileUserDto> GetFullUserProfileAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await _authService.GetToken());

        var response = await _httpClient.GetAsync("api/user/full-profile");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<FullProfileUserDto>();
        }

        throw new HttpRequestException("Failed to load user profile");
    }

    public async Task<bool> UpdatePremiumStatusAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await _authService.GetToken());

        var response = await _httpClient.GetAsync("api/User/premium");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateUserProfileAsync(UpdateUserProfileRequest request)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await _authService.GetToken());

        var response = await _httpClient.PutAsJsonAsync("api/user/update", request);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> IsUserPremiumAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await _authService.GetToken());

        var response = await _httpClient.GetAsync("api/User/isPremium");

        return response.IsSuccessStatusCode;
    }
}