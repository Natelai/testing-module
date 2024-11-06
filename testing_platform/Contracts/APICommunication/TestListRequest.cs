using Contracts.APICommunication.Enums;

namespace Contracts.APICommunication;

public class TestListRequest
{
    public PagedRequest PagedRequest { get; set; }
    public TestListOrderingDto TestListOrdering { get; set; }
    public TestCategory TestCategory { get; set; }
    public TestDifficulty? TestDifficulty { get; set; }
    public List<string>? Tags { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsFavourite { get; set; }
    public bool IsPremium { get; set; }
}