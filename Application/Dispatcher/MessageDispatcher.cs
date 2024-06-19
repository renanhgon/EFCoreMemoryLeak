using Application.Commands.Base;
using Application.Commands.Notification.AddNotificationMessage;
using Application.Commands.Notification.AddNotificationValidationResult;
using Domain.Notification;
using FluentValidation;
using Infra.Queries.Base;
using MediatR;

namespace Application.Dispatcher;

public class MessageDispatcher(IServiceProvider serviceProvider,
    IMediator mediator,
    INotificationReader notificationReader) : IMessageDispatcher
{
    public const string COMMAND_NULL_ERROR_MESSAGE = "Command cannot be null";
    public const string QUERY_NULL_ERROR_MESSAGE = "Query cannot be null";

    public async Task<TResult> DispatchCommandAsync<TResult>(Command<TResult> command, CancellationToken cancellationToken = default)
    {
        if (command is null)
            await mediator.Send(new AddNotificationMessageCommand(COMMAND_NULL_ERROR_MESSAGE), cancellationToken);
        else
            await ValidateAsync(command, cancellationToken);

        if (notificationReader.IsValid)
            return await mediator.Send(command!, cancellationToken);

        return default!;
    }

    public async Task<TResult> DispatchQueryAsync<TResult>(Query<TResult> query, CancellationToken cancellationToken = default)
    {
        if (query is null)
        {
            await mediator.Send(new AddNotificationMessageCommand(QUERY_NULL_ERROR_MESSAGE), cancellationToken);
            return default!;
        }

        return await mediator.Send(query, cancellationToken);
    }

    public async Task ValidateAsync<TResult>(Command<TResult> command, CancellationToken cancellationToken)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(command.GetType());
        var validator = (IValidator)serviceProvider.GetService(validatorType)!;
        if (validator is null) return;

        var validationContextType = typeof(ValidationContext<>).MakeGenericType(command.GetType());
        var validationContext = (IValidationContext)Activator.CreateInstance(validationContextType, command)!;

        var validationResult = await validator.ValidateAsync(validationContext, cancellationToken);
        if (!validationResult.IsValid)
            await mediator.Send(new AddNotificationsFromValidationResultCommand(validationResult), cancellationToken);
    }
}