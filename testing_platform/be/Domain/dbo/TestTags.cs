namespace Domain.dbo;

public class TestTags
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public int TagId { get; set; }

    public virtual Test Test { get; set; }
    public virtual Tag Tag { get; set; }
}