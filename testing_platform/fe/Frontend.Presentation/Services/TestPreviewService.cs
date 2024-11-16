using Contracts.APICommunication;
using Contracts.Internal;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Frontend.Presentation.Services;

public class TestPreviewService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public TestPreviewService(HttpClient httpClient, ApiSettings apiSettings, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<TestListResponse> GetPagedTestList(TestListRequest request)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await _authService.GetToken());

        var response = await _httpClient.PostAsJsonAsync("api/tests/list", request);

        if (response.IsSuccessStatusCode)
        {
            var rsp = new TestListResponse();

            return await response.Content.ReadFromJsonAsync<TestListResponse>();
        }

        throw new HttpRequestException("Failed to load test list.");
    }

    public async Task<TagsListResponse> GetTagsList(int limit, int offset, string? filter)
    {
        var httpclient = _httpClient;

        httpclient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await _authService.GetToken());

        var query = new Dictionary<string, string>
        {
            { "PagedRequest.Offset", offset.ToString() },
            { "PagedRequest.Limit", limit.ToString() }
        };

        if (!string.IsNullOrEmpty(filter))
        {
            query.Add("Filter", filter);
        }

        var queryString = string.Join("&", query.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        var response = await httpclient.GetAsync($"api/tests/tags?{queryString}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TagsListResponse>();
        }

        throw new HttpRequestException("Failed to load tags list.");
    }

    
}