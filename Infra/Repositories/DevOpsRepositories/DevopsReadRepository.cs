using DataTransferObjects;
using Infra.Context;
using Infra.DevOpsQueries;
using Infra.Queries.Base;

namespace Infra.Repositories.DevOpsRepositories;

public class DevopsReadRepository(IMyDbContext context)
    : IQueryHandler<PingQuery, StatusCheckDto>
{
    public async Task<StatusCheckDto> Handle(PingQuery request, CancellationToken cancellationToken)
    {
        string message;
        bool successConnection;

        try
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            successConnection = true;
            message = $"Transaction Id: {transaction.TransactionId}";
        }
        catch (Exception ex)
        {
            successConnection = false;
            message = ex.Message;
        }

        return new StatusCheckDto("hello",
            DateTime.Now,
            DateTime.Now,
            Guid.NewGuid(),
            successConnection,
            message);
    }
}