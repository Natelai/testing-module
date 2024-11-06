namespace DTOs.Requests.Tags;

public class TagsListRequest
{
    public required PagedRequest PagedRequest { get; set; }
    public string? Filter { get; set; }
}