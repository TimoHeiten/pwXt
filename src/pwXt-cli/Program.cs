using CliFx;
using heitech.pwXtCli.Commands;
using heitech.pwXtCli.Options;
using heitech.pwXtCli.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace heitech.pwXtCli;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        services.Configure<PwXtOptions>(config.GetSection("Config"));

        services.AddTransient<GetPassword>();
        services.AddTransient<ListPasswords>();
        services.AddTransient<TestEncryption>();

        services.AddTransient<MutatePassword>();
        services.AddTransient<CreatePasswordStore>();

        services.AddScoped<IPasswordStore, LiteDbStore>();

        return await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .UseTypeActivator(services.BuildServiceProvider())
            .Build()
            .RunAsync();
    }
}