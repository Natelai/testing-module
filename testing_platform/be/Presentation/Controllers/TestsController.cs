﻿using Domain.dbo;
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
    public async Task<ActionResult<List<TestPreviewDto>>> GetPagedTestList([FromBody] TestListRequest request, CancellationToken ct)
    {
        var user = await GetUserAsync(ct);
        var query = _context.Tests.AsQueryable();

        query = ApplyCategoryFilter(query, request.TestCategory);
        query = await ApplyFavouriteFilterAsync(query, request.IsFavourite, user.Id, ct);
        query = ApplyDifficultyFilter(query, request.Difficulties);
        query = ApplyTagFilter(query, request.Tags);
        query = query.Where(x => x.DurationInMinutes <= request.MaxDuration);

        if (request.DateOfTest != DateOnly.MinValue)
        {
            query = query.Where(x => x.UploadDate == request.DateOfTest);
        }

        query = ApplyAccesses(query, request.Accesses);
        query = await ApplyStatuses(query, request.TestStatuses, user.Id, ct);

        query = ApplyOrdering(query, request.TestSortBy);
        query = query.ApplyPagination(request.PagedRequest);

        var result = await ProjectToTestPreviewDto(query, user.Id, ct);
        return Ok(result);
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

    private IQueryable<Test> ApplyDifficultyFilter(IQueryable<Test> query, List<TestComplexity>? difficulty) =>
        difficulty.Count > 0 ? query.Where(t => difficulty.Contains(t.Complexity)) : query;

    private IQueryable<Test> ApplyTagFilter(IQueryable<Test> query, List<string> tags) =>
        tags != null && tags.Any()
            ? query.Where(t => t.TestTags.Any(tt => tags.Contains(tt.Tag.Name)))
            : query;

    private IQueryable<Test> ApplyOrdering(IQueryable<Test> query, TestSortBy ordering) =>
        ordering switch
        {
            TestSortBy.Newest => query.OrderBy(t => t.UploadDate),
            TestSortBy.Oldest => query.OrderByDescending(t => t.UploadDate),
            TestSortBy.Shortest => query.OrderBy(t => t.DurationInMinutes),
            TestSortBy.Longest => query.OrderByDescending(t => t.DurationInMinutes),
            _ => query
        };

    private IQueryable<Test> ApplyAccesses(IQueryable<Test> query, List<TestAccess> ordering)
    {
        if (ordering.Count == 2)
        {
            return query;
        }

        if (ordering.Contains(TestAccess.Free))
        {
            query = query.Where(x => x.IsPremium == false);
        }

        if (ordering.Contains(TestAccess.Premium))
        {
            query = query.Where(x => x.IsPremium == true);
        }

        return query;
    }

    private async Task<IQueryable<Test>> ApplyStatuses(IQueryable<Test> query, List<TestStatus> ordering, int userId, CancellationToken ct)
    {
        if (ordering.Contains(TestStatus.Completed))
        {
            query = await ApplyCompletedFilterAsync(query, true, userId, ct);
        }

        return query;
    }

    private async Task<List<TestPreviewDto>> ProjectToTestPreviewDto(IQueryable<Test> query, int userId, CancellationToken ct) =>
        await query.Select(x => new TestPreviewDto
        {
            Category = x.Category.ToString(),
            Caption = x.Caption,
            Complexity = x.Complexity.ToString(),
            DurationInMinutes = x.DurationInMinutes,
            IsCompleted = _context.CompletedTests.Any(ct => ct.TestId == x.Id && ct.UserId == userId),
            IsFavourite = _context.FavouriteTests.Any(ft => ft.TestId == x.Id && ft.UserId == userId),
            IsPremium = x.IsPremium,
            Tags = x.TestTags.Select(tt => tt.Tag.Name).ToList(),
            UploadDate = x.UploadDate,
            Id = x.Id,
        }).ToListAsync(ct);

}