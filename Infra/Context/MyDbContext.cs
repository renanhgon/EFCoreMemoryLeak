using Config.Database;
using Domain.Entities;
using Infra.Context.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Infra.Context;

public class MyDbContext : DbContext, IMyDbContext
{
    private readonly IDatabaseConfig _configuration;
    private readonly Guid _contextId;
    private readonly ILogger<MyDbContext> _logger;
    private readonly ISeed _seed;

    public MyDbContext(IDatabaseConfig configuration, ISeed seed, ILogger<MyDbContext> logger)
    {
        _contextId = Guid.NewGuid();
        _logger = logger;

        _logger.LogInformation("Context created. ID {0}", _contextId.ToString());

        _configuration = configuration;
        _seed = seed;
    }

    public bool ExistsPendingEntitiesToSave
       => ChangeTracker.Entries().Any(entity => entity.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

    public DbSet<User> Users { get; set; }

    public bool AllMigrationsApplied()
    {
        var migrationsAlreadyExecutedIds = this.GetService<IHistoryRepository>()
            .GetAppliedMigrations()
            .Select(m => m.MigrationId);

        var allMigrationIds = this.GetService<IMigrationsAssembly>()
            .Migrations
            .Select(m => m.Key);

        return !allMigrationIds.Except(migrationsAlreadyExecutedIds).Any();
    }

    public void ClearChangeTracker()
        => ChangeTracker.Clear();

    public override void Dispose()
    {
        base.Dispose();
        _logger.LogInformation("Context destroyed. ID {0}", _contextId.ToString());
    }

    public override ValueTask DisposeAsync()
    {
        var result = base.DisposeAsync();
        _logger.LogInformation("Context destroyed. ID {0}", _contextId.ToString());
        return result;
    }

    public List<T> GetTrackedItemsOfType<T>(params EntityState[] states)
        => ChangeTracker.Entries()
            ?.Where(x => x.Entity is T && states.Contains(x.State))
            ?.Select(x => x.Entity)
            ?.Cast<T>()
            ?.ToList();

    public virtual void MigrateDatabase()
    {
        var pendingMigrations = Database.GetPendingMigrations();
        if (pendingMigrations != null && pendingMigrations.Any())
            Database.Migrate();
    }

    public void Seed() => _seed.Execute(this);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(_configuration.ConnectionString);

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

        HandleStringProps(modelBuilder);
        HandleDateTimeProps(modelBuilder);
    }

    private static void HandleDateTimeProps(ModelBuilder modelBuilder)
    {
        var dateTimeProps = modelBuilder.Model.GetEntityTypes()
            .SelectMany(x => x.GetProperties())
            .Where(x => x.PropertyInfo?.PropertyType == typeof(DateTime) || x.PropertyInfo?.PropertyType == typeof(DateTime?))
            .ToList();

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => DateTime.SpecifyKind(v.ToUniversalTime(), DateTimeKind.Unspecified),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? DateTime.SpecifyKind(v.Value.ToUniversalTime(), DateTimeKind.Unspecified) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var prop in dateTimeProps)
        {
            prop.SetColumnType("timestamp");

            if (prop.PropertyInfo!.PropertyType == typeof(DateTime))
                prop.SetValueConverter(dateTimeConverter);
            else if (prop.PropertyInfo.PropertyType == typeof(DateTime?))
                prop.SetValueConverter(nullableDateTimeConverter);
        }
    }

    private static void HandleStringProps(ModelBuilder modelBuilder)
    {
        var stringProps = modelBuilder.Model.GetEntityTypes()
            .SelectMany(x => x.GetProperties())
            .Where(x => x.PropertyInfo?.PropertyType == typeof(string) && x.GetColumnType() != "jsonb")
            .ToList();

        const int defaultMaxLength = 255;
        foreach (var prop in stringProps)
        {
            var maxLength = prop.GetMaxLength();
            if (maxLength == null || maxLength <= 0 || maxLength > defaultMaxLength)
                prop.SetMaxLength(defaultMaxLength);
        }
    }
}