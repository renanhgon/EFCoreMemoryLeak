using Application.Commands.Base;

using Infra.Queries.Base;

namespace Application.Dispatcher;

public interface IMessageDispatcher
{
    Task<TResult> DispatchCommandAsync<TResult>(Command<TResult> command, CancellationToken cancellationToken = default);

    Task<TResult> DispatchQueryAsync<TResult>(Query<TResult> query, CancellationToken cancellationToken = default);

    Task ValidateAsync<TResult>(Command<TResult> command, CancellationToken cancellationToken);
}