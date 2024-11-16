using Domain.Enums;

namespace DTOs.Requests.Tests;

public class TestListRequest
{
    public List<TestComplexity> Difficulties { get; set; } = [];
    public List<string>? Tags { get; set; } = [];
    public List<TestAccess> Accesses { get; set; } = [];
    public List<TestStatus> TestStatuses { get; set; } = [];
    public string Search { get; set; }
    public DateOnly DateOfTest { get; set; }
    public int MaxDuration { get; set; } = 10;
    public TestSortBy TestSortBy { get; set; }
    public bool IsFavourite { get; set; }
    public TestCategory TestCategory { get; set; }
    public PagedRequest PagedRequest { get; set; }
}