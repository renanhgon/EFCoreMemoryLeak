using DataTransferObjects;

namespace Application.Service;

public interface IDevOpsService
{
    Task<StatusCheckDto> GetIntegridadeDoStatusCheckAsync();
}