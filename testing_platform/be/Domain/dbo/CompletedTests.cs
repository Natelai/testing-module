namespace Domain.dbo;

public class CompletedTests
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public int UserId { get; set; }

    public virtual Test Test { get; set; }
    public virtual User User { get; set; }
}