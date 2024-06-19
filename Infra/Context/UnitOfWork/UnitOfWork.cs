using Auth.Providers.User;
using Domain.Entities;
using Domain.Entities.Base;
using Domain.Entities.Base.ByUser;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Infra.Context.UnitOfWork
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        public const string COMMIT_INITIALIZED_NOT_STARTED_ERROR_MESSAGE = "Cannot commit the unit of work because it has not been started";
        public const string EXISTS_PENDING_ENTITIES_TO_SAVE_ERROR_MESSAGE = "Cannot start the unit of work because there are already modified entities";
        public const string INITIALIZED_STARTED_ERROR_MESSAGE = "Cannot start the unit of work because it has already been started";
        public const string ROLLBACK_INITIALIZED_NOT_STARTED_ERROR_MESSAGE = "Cannot rollback the unit of work because it has not been started";
        private const int MAX_DEGREE_OF_PARALLELISM = 5;

        private readonly IMyDbContext _context;

        private readonly IUserAuthProvider _userAuthProvider;

        public UnitOfWork(
            IUserAuthProvider userAuthProvider,
            IMyDbContext context)
        {
            _context = context;
            _userAuthProvider = userAuthProvider;
        }

        public bool Initialized { get; private set; }

        public int Commit()
        {
            if (!Initialized)
                throw new TransactionException(COMMIT_INITIALIZED_NOT_STARTED_ERROR_MESSAGE);

            HandleEntities().Wait();
            return _context.SaveChanges();
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            if (!Initialized)
                throw new TransactionException(COMMIT_INITIALIZED_NOT_STARTED_ERROR_MESSAGE);

            await HandleEntities();
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(Initialized);
            GC.SuppressFinalize(this);
        }

        public void Rollback()
        {
            if (!Initialized)
                throw new TransactionException(ROLLBACK_INITIALIZED_NOT_STARTED_ERROR_MESSAGE);

            _context.ClearChangeTracker();
        }

        public IUnitOfWork Start()
        {
            if (Initialized)
                throw new TransactionException(INITIALIZED_STARTED_ERROR_MESSAGE);

            if (_context.ExistsPendingEntitiesToSave)
                throw new TransactionException(EXISTS_PENDING_ENTITIES_TO_SAVE_ERROR_MESSAGE);

            Initialized = true;
            return this;
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context.ExistsPendingEntitiesToSave)
                    Rollback();

                Initialized = false;
            }
        }

        private async Task HandleEntities()
        {
            SetUpdatedAtOnModifiedEntities();
            if (!_userAuthProvider.IsAuthenticated) return;

            var user = _context.GetTrackedItemsOfType<User>().Find(x => x.Id == _userAuthProvider.UserId)
                ?? await _context.Users.FirstOrDefaultAsync(x => x.Id == _userAuthProvider.UserId);

            SetCreatedByUserIdOnAddedEntity(user);
            SetUpdatedByUserIdOnModifiedEntities(user);
        }

        private void SetCreatedByUserIdOnAddedEntity(User user)
        {
            var addedEntities = _context.GetTrackedItemsOfType<EntityByUser>(EntityState.Added) ?? [];

            Parallel.ForEach(
                addedEntities,
                new ParallelOptions { MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM },
                entity => entity.SetCreatedByUser(user));
        }

        private void SetUpdatedAtOnModifiedEntities()
        {
            var updatedEntities = _context.GetTrackedItemsOfType<Entity>(EntityState.Modified) ?? [];
            var updateDateTime = DateTime.UtcNow;

            Parallel.ForEach(
                updatedEntities,
                new ParallelOptions { MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM },
                entity => entity.SetUpdatedAt(updateDateTime));
        }

        private void SetUpdatedByUserIdOnModifiedEntities(User user)
        {
            var updatedEntities = _context.GetTrackedItemsOfType<EntityByUser>(EntityState.Modified) ?? [];

            Parallel.ForEach(
                updatedEntities,
                new ParallelOptions { MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM },
                entity => entity.SetUpdatedByUser(user));
        }
    }
}