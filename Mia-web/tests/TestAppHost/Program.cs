using Mia_Web.Shared;

namespace Mia_Web.TestAppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        builder
            .AddSqlite(Services.Database);

        builder.Build().Run();
    }
}