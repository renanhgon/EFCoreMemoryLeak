namespace Infra.Context.Seed
{
    public interface ISeed
    {
        void Execute(IMyDbContext context);
    }
}