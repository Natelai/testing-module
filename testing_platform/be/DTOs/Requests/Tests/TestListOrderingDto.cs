using FluentValidation;

namespace DTOs.Requests.Tests;

public class TestListOrderingDto
{
    public bool OrderByDate { get; set; }
    public bool OrderByDuration { get; set; }
    public bool OrderInAscOrder { get; set; }
}

public class TestListOrderingDtoValidator : AbstractValidator<TestListOrderingDto>
{
    public TestListOrderingDtoValidator()
    {
        RuleFor(x => x)
            .Must(x => !(x.OrderByDate && x.OrderByDuration))
            .WithMessage("You can only select one sorting criteria: either by date or by duration.");

        RuleFor(x => x.OrderInAscOrder)
            .Must(orderInAscOrder => true)
            .When(x => x.OrderByDate || x.OrderByDuration)
            .WithMessage("You must specify the order direction when sorting criteria is selected.");
    }
}