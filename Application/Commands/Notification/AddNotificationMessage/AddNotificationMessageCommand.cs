using Application.Commands.Base;
using Domain.Constants;
using FluentValidation;

using MediatR;

namespace Application.Commands.Notification.AddNotificationMessage
{
    public class AddNotificationMessageCommand : Command<Unit>
    {
        public AddNotificationMessageCommand(string message, string errorCode = ErrorCodes.GENERIC, Severity severity = Severity.Error)
        {
            Message = message;
            ErrorCode = errorCode;
            Severity = severity;
        }

        public string ErrorCode { get; private set; }
        public string Message { get; private set; }
        public Severity Severity { get; private set; }
    }
}