using Domain.Enums;
using FluentValidation;

namespace DTOs.Requests.Tests;

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

public class TestListRequestValidator : AbstractValidator<TestListRequest>
{
    public TestListRequestValidator()
    {
        RuleFor(x => x.PagedRequest)
            .NotNull()
            .WithMessage("PagedRequest is required.")
            .SetValidator(new PagedRequestValidator());

        RuleFor(x => x.TestListOrdering)
            .NotNull()
            .WithMessage("TestListOrdering is required.")
            .SetValidator(new TestListOrderingDtoValidator());

        RuleFor(x => x.TestCategory)
            .IsInEnum()
            .WithMessage("Invalid value for TestCategory.");

        RuleFor(x => x.TestDifficulty)
            .IsInEnum()
            .WithMessage("Invalid value for TestDifficulty.");
    }
}