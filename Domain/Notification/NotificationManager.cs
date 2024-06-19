using Domain.Constants;
using FluentValidation;
using FluentValidation.Results;
using System.Net;

namespace Domain.Notification
{
    public class NotificationManager : ValidationResult, INotificationWriter, INotificationReader
    {
        private const string NO_FIELD_PROPERTY_NAME = "NO_FIELD";

        public string ErrorCode
        {
            get
            {
                if (IsValid) return null;

                if (Errors.Any(error => error.ErrorCode == ErrorCodes.EXCEPTION)) return ErrorCodes.EXCEPTION;
                if (Errors.Any(error => error.ErrorCode == ErrorCodes.UNAUTHORIZE)) return ErrorCodes.UNAUTHORIZE;
                if (Errors.Any(error => error.ErrorCode == ErrorCodes.FORBIDDEN)) return ErrorCodes.FORBIDDEN;
                if (Errors.Any(error => error.ErrorCode == ErrorCodes.TIMEOUT)) return ErrorCodes.TIMEOUT;
                if (Errors.Any(error => error.ErrorCode == ErrorCodes.NOT_FOUND)) return ErrorCodes.NOT_FOUND;
                if (Errors.Any(error => error.ErrorCode == ErrorCodes.UNPROCESSABLE_ENTITY)) return ErrorCodes.UNPROCESSABLE_ENTITY;

                return ErrorCodes.GENERIC;
            }
        }

        public IReadOnlyCollection<ValidationFailure> FieldMessages
            => GetFieldMessages();

        public IReadOnlyDictionary<string, string[]> FieldMessagesDictionary
            => ToDictionary()
                .Where(field => !string.IsNullOrWhiteSpace(field.Key) && field.Key != NO_FIELD_PROPERTY_NAME)
                .ToDictionary(x => x.Key, x => x.Value);

        public IReadOnlyCollection<string> MessagesList
            => GetMessagesList();

        public IReadOnlyCollection<ValidationFailure> MessagesWithoutField
            => GetMessagesWithoutField();

        public HttpStatusCode StatusCode
        {
            get
            {
                if (IsValid) return HttpStatusCode.OK;

                return ErrorCode switch
                {
                    ErrorCodes.EXCEPTION => HttpStatusCode.InternalServerError,
                    ErrorCodes.UNAUTHORIZE => HttpStatusCode.Unauthorized,
                    ErrorCodes.FORBIDDEN => HttpStatusCode.Forbidden,
                    ErrorCodes.TIMEOUT => HttpStatusCode.RequestTimeout,
                    ErrorCodes.NOT_FOUND => HttpStatusCode.NotFound,
                    ErrorCodes.UNPROCESSABLE_ENTITY => HttpStatusCode.UnprocessableEntity,
                    _ => HttpStatusCode.BadRequest
                };
            }
        }

        public void AddFieldMessage(string fieldName, string message, string errorCode = ErrorCodes.GENERIC, Severity severity = Severity.Error)
            => Errors = Errors.Append(new ValidationFailure(fieldName, message) { Severity = severity, ErrorCode = errorCode }).ToList();

        public void AddMessage(string message, string errorCode = ErrorCodes.GENERIC, Severity severity = Severity.Error)
            => Errors = Errors.Append(new ValidationFailure(NO_FIELD_PROPERTY_NAME, message) { Severity = severity, ErrorCode = errorCode }).ToList();

        public void AddValidationResult(ValidationResult validationResult, bool IncludeFieldsOnMessages = true)
            => validationResult?.Errors?.ForEach(error =>
            {
                var code = string.IsNullOrWhiteSpace(error.ErrorCode) ? ErrorCodes.GENERIC : error.ErrorCode;
                var severity = error.Severity == default ? Severity.Error : error.Severity;

                if (IncludeFieldsOnMessages)
                    AddFieldMessage(error.PropertyName, error.ErrorMessage, code, severity);
                else
                    AddMessage(error.ErrorMessage, code, severity);
            });

        private IReadOnlyCollection<ValidationFailure> GetFieldMessages()
            => Errors.Where(msg => msg.PropertyName != NO_FIELD_PROPERTY_NAME).ToList();

        private List<string> GetMessagesList()
            => Errors.Select(error => $"{(error.PropertyName != NO_FIELD_PROPERTY_NAME ? $"{error.PropertyName}: " : "")}{error.ErrorMessage}").ToList();

        private List<ValidationFailure> GetMessagesWithoutField()
            => Errors.Where(msg => msg.PropertyName == NO_FIELD_PROPERTY_NAME).ToList();
    }
}