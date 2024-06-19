using Application.Commands.Base;
using Domain.Notification;
using MediatR;

namespace Application.Commands.Notification.AddNotificationMessage
{
    public class AddNotificationMessageCommandHandler(INotificationWriter notificationWriter)
        : CommandHandler<AddNotificationMessageCommand, Unit>
    {
        public override Task<Unit> Handle(AddNotificationMessageCommand request, CancellationToken cancellationToken)
        {
            notificationWriter.AddMessage(request.Message, request.ErrorCode, request.Severity);
            return Task.FromResult(Unit.Value);
        }
    }
}