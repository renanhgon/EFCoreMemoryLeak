namespace DataTransferObjects;

public record StatusCheckDto(string AvailabilityZone,
    DateTime? DateTimeDatabase,
    DateTime? DateTimeServer,
    Guid InstanceId,
    bool IsOk,
    string Message);