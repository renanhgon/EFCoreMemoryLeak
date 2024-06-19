using Domain.Constants;
using FluentValidation;
using FluentValidation.Results;

namespace Domain.Notification
{
    public interface INotificationWriter
    {
        void AddFieldMessage(string fieldName, string message, string errorCode = ErrorCodes.GENERIC, Severity severity = Severity.Error);

        void AddMessage(string message, string errorCode = ErrorCodes.GENERIC, Severity severity = Severity.Error);

        void AddValidationResult(ValidationResult validationResult, bool IncludeFieldsOnMessages = true);
    }
}