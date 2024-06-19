using Application.Commands.Base;
using Domain.Notification;
using MediatR;

namespace Application.Commands.Notification.AddNotificationFieldMessage
{
    public class AddNotificationFieldMessageCommandHandler(INotificationWriter notificationWriter) : CommandHandler<AddNotificationFieldMessageCommand, Unit>
    {
        public override Task<Unit> Handle(AddNotificationFieldMessageCommand request, CancellationToken cancellationToken)
        {
            notificationWriter.AddFieldMessage(request.FieldName, request.Message, request.ErrorCode, request.Severity);
            return Task.FromResult(Unit.Value);
        }
    }
}