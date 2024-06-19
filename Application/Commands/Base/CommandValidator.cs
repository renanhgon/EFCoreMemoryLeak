using FluentValidation;
using System.ComponentModel;
using System.Reflection;

namespace Application.Commands.Base
{
    public abstract class CommandValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : class, ICommand
    {
        public static string GetDisplayNameOrDefault(string fieldName)
            => typeof(TCommand).GetProperty(fieldName)?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? fieldName;
    }
}