using Application.Dispatcher;
using DataTransferObjects;
using Infra.DevOpsQueries;

namespace Application.Service;

public class DevOpsService(IMessageDispatcher messageDispatcher) : IDevOpsService
{
    public Task<StatusCheckDto> GetIntegridadeDoStatusCheckAsync()
       => messageDispatcher.DispatchQueryAsync(new PingQuery());
}