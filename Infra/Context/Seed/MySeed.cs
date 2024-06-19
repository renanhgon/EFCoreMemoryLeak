namespace Infra.Context.Seed;

public class MySeed : ISeed
{
    public virtual void Execute(IMyDbContext context)
    {
        //new ExampleSeed(context).Execute();
        //new AnotherExampleSeed(context).Execute();
    }
}