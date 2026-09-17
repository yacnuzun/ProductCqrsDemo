using FluentValidation;

namespace ProductCqrsDemo.Api.Modules.Products.Application.Validators
{
    public sealed class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ürün adı zorunludur.")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Fiyat sıfırdan büyük olmalıdır.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz.");
        }
    }
}
