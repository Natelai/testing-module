using FluentValidation;

namespace DTOs.Requests;

public class PagedRequest
{
    public int Offset { get; set; }
    public int Limit { get; set; }
}

public class PagedRequestValidator : AbstractValidator<PagedRequest>
{
    public PagedRequestValidator()
    {
        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Offset must be a non-negative integer.");

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .WithMessage("Limit must be greater than zero.")
            .LessThanOrEqualTo(100)
            .WithMessage("Limit must be less than or equal to 100.");
    }
}