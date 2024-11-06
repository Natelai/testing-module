namespace Contracts.APICommunication;

public class TestPreviewDto
{
    public int Id { get; set; }
    public bool IsPremium { get; set; }
    public string Caption { get; set; } = null!;
    public DateOnly UploadDate { get; set; }
    public byte DurationInMinutes { get; set; }
    public string Category { get; set; } = null!;
    public string Complexity { get; set; } = null!;
    public List<string> Tags { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public bool IsFavourite { get; set; }
}