using Application.Commands.Base;
using Domain.Notification;
using MediatR;

namespace Application.Commands.Notification.AddNotificationValidationResult
{
    public class AddNotificationsFromValidationResultCommandHandler(INotificationWriter notificationWriter)
        : CommandHandler<AddNotificationsFromValidationResultCommand, Unit>
    {
        public override Task<Unit> Handle(AddNotificationsFromValidationResultCommand request, CancellationToken cancellationToken)
        {
            notificationWriter.AddValidationResult(request.ValidationResult, request.IncludeFieldsOnMessages);
            return Task.FromResult(Unit.Value);
        }
    }
}