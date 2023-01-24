using CliFx;
using heitech.pwXtCli.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace heitech.pwXtCli;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables("PW_XT_")
            .Build();

        var options = config.GetSection("Config");

        string Configure(string key)
            => options.GetValue<string>(key);

        services.AddPwXtServices(configure => configure.WithPassphrase(Configure("Passphrase"))
            .WithSalt(Configure("Salt"))
            .WithConnectionString(Configure("ConnectionString"))
            .WithStoreType(Dependencies.StoreType.LiteDb));

        services.AddTransient<GetPassword>();
        services.AddTransient<ListPasswords>();
        // services.AddTransient<TestEncryption>();

        services.AddTransient<MutatePassword>();
        services.AddTransient<CreatePasswordStore>();

        return await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .UseTypeActivator(services.BuildServiceProvider())
            .Build()
            .RunAsync();
    }
}