namespace CleanArc.Application.Orders.Commands.CreateOrder;

using FluentValidation;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must have at least one item");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.ProductId)
                    .NotEmpty().WithMessage("Product ID is required");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than zero");

                item.RuleFor(x => x.UnitPrice)
                    .GreaterThan(0).WithMessage("Unit price must be greater than zero");
            });
    }
}
