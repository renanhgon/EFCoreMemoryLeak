using FluentValidation.Results;
using System.Net;

namespace Domain.Notification;

public interface INotificationReader
{
    string ErrorCode { get; }
    IReadOnlyCollection<ValidationFailure> FieldMessages { get; }
    IReadOnlyDictionary<string, string[]> FieldMessagesDictionary { get; }
    bool IsValid { get; }
    IReadOnlyCollection<string> MessagesList { get; }
    IReadOnlyCollection<ValidationFailure> MessagesWithoutField { get; }
    HttpStatusCode StatusCode { get; }
}