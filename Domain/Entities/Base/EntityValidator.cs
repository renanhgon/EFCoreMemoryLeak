using FluentValidation;
using System.ComponentModel;
using System.Reflection;

namespace Domain.Entities.Base;

public abstract class EntityValidator<TEntity> : AbstractValidator<TEntity>
        where TEntity : Entity
{
    private const string CREATED_AT_REQUIRED_ERROR_MESSAGE = "An entity must have a creation date";
    private const string ID_REQUIRED_ERROR_MESSAGE = "An entity must have an Id";

    protected EntityValidator()
    {
        ApplyRulesToId();
        ApplyRulesToCreatedAt();
    }

    public static string GetDisplayNameOrDefault(string fieldName)
        => typeof(TEntity).GetProperty(fieldName)?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? fieldName;

    private void ApplyRulesToCreatedAt()
    {
        RuleFor(entity => entity.CreatedAt).Cascade(CascadeMode.Stop)
        .NotEmpty()
        .WithMessage(CREATED_AT_REQUIRED_ERROR_MESSAGE);
    }

    private void ApplyRulesToId()
    {
        RuleFor(entity => entity.Id).Cascade(CascadeMode.Stop)
        .NotEmpty()
        .WithMessage(ID_REQUIRED_ERROR_MESSAGE);
    }
}