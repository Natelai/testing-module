using DTOs.Requests;
using DTOs.Requests.Tests;
using Infrastructure;

namespace Presentation.Extensions;

public static class DbContextExtensions
{
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PagedRequest req) where T : class 
        => query.Skip(req.Offset).Take(req.Limit);

}