using MediatR;

namespace Application.Commands.Base;

public abstract class Command<TResult> : ICommand, IRequest<TResult>
{
    protected Command() => DateTimeOfSent = DateTime.UtcNow;

    public DateTime DateTimeOfSent { get; private set; }
}