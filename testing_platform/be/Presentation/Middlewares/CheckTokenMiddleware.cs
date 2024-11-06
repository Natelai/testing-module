namespace Presentation.Middlewares;

public class CheckTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CheckTokenMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        var token = context?.Request?.Headers?["Authorization"].ToString();
        token = string.IsNullOrEmpty(token) ? context?.Request?.Query?["access_token"] : token;

        await _next.Invoke(context);
            
    }
}