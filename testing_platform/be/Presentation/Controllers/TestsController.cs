using Domain.dbo;
using Domain.Enums;
using DTOs.Requests.Tests;
using DTOs.Responses.Auth.Tests;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Extensions;
using System.Data;
using DTOs.Requests.Tags;
using DTOs.Responses.Tags;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public class TestsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpPost("list")]
    public async Task<ActionResult<TestListResponse>> GetPagedTestList([FromBody] TestListRequest request, CancellationToken ct)
    {
        var user = await GetUserAsync(ct);
        var query = _context.Tests.AsQueryable();

        query = ApplyCategoryFilter(query, request.TestCategory);
        query = ApplyPremiumFilter(query, request.IsPremium);
        query = await ApplyFavouriteFilterAsync(query, request.IsFavourite, user.Id, ct);
        query = await ApplyCompletedFilterAsync(query, request.IsCompleted, user.Id, ct);
        query = ApplyDifficultyFilter(query, request.TestDifficulty);
        query = ApplyTagFilter(query, request.Tags);
        query = ApplyOrdering(query, request.TestListOrdering);

        var count = await query.CountAsync(ct);

        query = query.ApplyPagination(request.PagedRequest);

        var result = await ProjectToTestPreviewDto(query, user.Id, ct);

        var rsp = new TestListResponse
        {
            Data = result,
            TotalCount = count,
        };

        return Ok(rsp);
    }

    [HttpGet("tags")]
    public async Task<ActionResult<TagsListResponse>> GetTagsList([FromQuery] TagsListRequest request,
        CancellationToken ct)
    {
        var rsp = new TagsListResponse();

        var query = _context.Tags.AsQueryable();

        if (!string.IsNullOrEmpty(request.Filter))
        {
            query = query.Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));
        }

        rsp.Data = await query
            .OrderBy(x => x.Name)
            .Skip(request.PagedRequest.Offset)
            .Take(request.PagedRequest.Limit)
            .Select(x => x.Name).ToListAsync(ct);

        rsp.TotalCount = await query.CountAsync(ct);

        return Ok(rsp);
    }

    private async Task<User> GetUserAsync(CancellationToken ct) =>
        await _context.Users.SingleAsync(x => x.UserName == HttpContext.User!.Identity!.Name, ct);

    private IQueryable<Test> ApplyCategoryFilter(IQueryable<Test> query, TestCategory? category) =>
        category.HasValue ? query.Where(t => t.Category == category) : query;

    private IQueryable<Test> ApplyPremiumFilter(IQueryable<Test> query, bool isPremium) =>
        isPremium ? query.Where(t => t.IsPremium == true) : query;

    private async Task<IQueryable<Test>> ApplyFavouriteFilterAsync(IQueryable<Test> query, bool isFavourite, int userId, CancellationToken ct)
    {
        if (!isFavourite) return query;

        var favouriteTestIds = await _context.FavouriteTests
            .Where(ft => ft.UserId == userId)
            .Select(ft => ft.TestId)
            .ToListAsync(ct);

        return query.Where(t => favouriteTestIds.Contains(t.Id));
    }

    private async Task<IQueryable<Test>> ApplyCompletedFilterAsync(IQueryable<Test> query, bool isCompleted, int userId, CancellationToken ct)
    {
        if (!isCompleted) return query;

        var completedTestIds = await _context.CompletedTests
            .Where(ct => ct.UserId == userId)
            .Select(ct => ct.TestId)
            .ToListAsync(ct);

        return query.Where(t => completedTestIds.Contains(t.Id));
    }

    private IQueryable<Test> ApplyDifficultyFilter(IQueryable<Test> query, TestDifficulty? difficulty) =>
        difficulty.HasValue ? query.Where(t => t.Difficulty == difficulty) : query;

    private IQueryable<Test> ApplyTagFilter(IQueryable<Test> query, List<string> tags) =>
        tags != null && tags.Any()
            ? query.Where(t => t.TestTags.Any(tt => tags.Contains(tt.Tag.Name)))
            : query;

    private IQueryable<Test> ApplyOrdering(IQueryable<Test> query, TestListOrderingDto ordering) =>
        ordering switch
        {
            { OrderByDate: true, OrderInAscOrder: true } => query.OrderBy(t => t.UploadDate),
            { OrderByDate: true, OrderInAscOrder: false } => query.OrderByDescending(t => t.UploadDate),
            { OrderByDuration: true, OrderInAscOrder: true } => query.OrderBy(t => t.DurationInMinutes),
            { OrderByDuration: true, OrderInAscOrder: false } => query.OrderByDescending(t => t.DurationInMinutes),
            _ => query
        };

    private async Task<List<TestPreviewDto>> ProjectToTestPreviewDto(IQueryable<Test> query, int userId, CancellationToken ct) =>
        await query.Select(x => new TestPreviewDto
        {
            Category = x.Category.ToString(),
            Caption = x.Caption,
            Complexity = x.Difficulty.ToString(),
            DurationInMinutes = x.DurationInMinutes,
            IsCompleted = _context.CompletedTests.Any(ct => ct.TestId == x.Id && ct.UserId == userId),
            IsFavourite = _context.FavouriteTests.Any(ft => ft.TestId == x.Id && ft.UserId == userId),
            IsPremium = x.IsPremium,
            Tags = x.TestTags.Select(tt => tt.Tag.Name).ToList(),
            UploadDate = x.UploadDate,
            Id = x.Id,
        }).ToListAsync(ct);

}