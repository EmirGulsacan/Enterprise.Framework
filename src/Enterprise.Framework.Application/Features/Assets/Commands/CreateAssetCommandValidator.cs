namespace Enterprise.Framework.Application.Features.Assets.Commands;

using Enterprise.Framework.Domain.Common.Enums;
using FluentValidation;

public class CreateAssetCommandValidator : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Varlık adı boş olamaz.")
            .MaximumLength(200).WithMessage("Varlık adı 200 karakteri geçemez.");

        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("Seri numarası boş olamaz.")
            .MaximumLength(100).WithMessage("Seri numarası 100 karakteri geçemez.");

        RuleFor(x => x.PurchaseDate)
            .NotEmpty().WithMessage("Satın alma tarihi zorunludur.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Satın alma tarihi gelecekte olamaz.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Geçersiz varlık durumu.");

        RuleFor(x => x.AssignedEmployeeId)
            .GreaterThan(0).When(x => x.AssignedEmployeeId.HasValue)
            .WithMessage("Çalışan ID geçerli bir değer olmalıdır.");
    }
}
