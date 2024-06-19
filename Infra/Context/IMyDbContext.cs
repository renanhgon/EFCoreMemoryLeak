using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infra.Context;

public interface IMyDbContext : IDisposable
{
    DbSet<User> Users { get; set; }

    DatabaseFacade Database { get; }

    bool ExistsPendingEntitiesToSave { get; }

    bool AllMigrationsApplied();

    void ClearChangeTracker();

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    List<T> GetTrackedItemsOfType<T>(params EntityState[] states);

    void MigrateDatabase();

    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    void Seed();

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}