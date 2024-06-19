using MediatR;

namespace Infra.Queries.Base
{
    public abstract class Query<TResult> : IQuery, IRequest<TResult>
    {
        protected Query() => DateTimeOfSent = DateTime.UtcNow;

        public DateTime DateTimeOfSent { get; private set; }
    }
}