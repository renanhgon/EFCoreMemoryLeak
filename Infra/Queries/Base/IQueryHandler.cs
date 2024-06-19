using MediatR;

namespace Infra.Queries.Base
{
    public interface IQueryHandler<TQuery, TQueryResult> : IRequestHandler<TQuery, TQueryResult>
        where TQuery : Query<TQueryResult>
    {
    }
}