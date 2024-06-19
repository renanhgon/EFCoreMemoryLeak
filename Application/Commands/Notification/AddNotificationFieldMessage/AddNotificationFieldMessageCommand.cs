using Application.Commands.Base;
using Domain.Constants;
using FluentValidation;
using MediatR;

namespace Application.Commands.Notification.AddNotificationFieldMessage
{
    public class AddNotificationFieldMessageCommand : Command<Unit>
    {
        public AddNotificationFieldMessageCommand(string fieldName, string message,
            string errorCode = ErrorCodes.GENERIC, Severity severity = Severity.Error)
        {
            FieldName = fieldName;
            Message = message;
            ErrorCode = errorCode;
            Severity = severity;
        }

        public string ErrorCode { get; private set; }
        public string FieldName { get; private set; }
        public string Message { get; private set; }
        public Severity Severity { get; private set; }
    }
}