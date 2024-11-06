namespace Contracts.APICommunication;

public class TagsListRequest
{
    public required PagedRequest PagedRequest { get; set; }
    public string? Filter { get; set; }
}