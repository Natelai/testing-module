namespace Contracts.APICommunication;

public class TestListResponse
{
    public List<TestPreviewDto> Data { get; set; }
    public int TotalCount { get; set; }
}