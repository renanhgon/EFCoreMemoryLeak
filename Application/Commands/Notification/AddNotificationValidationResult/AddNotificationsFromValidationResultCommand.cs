using Application.Commands.Base;
using FluentValidation.Results;
using MediatR;

namespace Application.Commands.Notification.AddNotificationValidationResult
{
    public class AddNotificationsFromValidationResultCommand : Command<Unit>
    {
        public AddNotificationsFromValidationResultCommand(ValidationResult validationResult, bool includeFieldsOnMessages = true)
        {
            ValidationResult = validationResult;
            IncludeFieldsOnMessages = includeFieldsOnMessages;
        }

        public bool IncludeFieldsOnMessages { get; private set; }
        public ValidationResult ValidationResult { get; private set; }
    }
}