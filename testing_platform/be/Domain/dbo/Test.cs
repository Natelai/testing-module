using Domain.Enums;

namespace Domain.dbo;

public class Test
{
    public int Id { get; set; }
    public string Caption { get; set; }
    public string Description { get; set; }
    public byte DurationInMinutes { get; set; }
    public DateOnly UploadDate { get; set; }
    public TestCategory Category { get; set; }
    public TestDifficulty Difficulty { get; set; }
    public bool IsPremium { get; set; }

    public virtual List<TestTags> TestTags { get; set; }
    public virtual List<CompletedTests> CompletedTests { get; set; }
    public virtual List<FavouriteTests> FavouriteTests { get; set; }
}