using heitech.pwXtCli.Options;
using pwXt_Service.Commands;
using pwXt_Service.Services;
using pwXt_Service.Store;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class Dependencies
{
    /// <summary>
    /// Add all dependencies for the password manager Backend
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddPwXtServices(this IServiceCollection services, Func<PasswordStoreBuilder, PasswordStoreBuilder> configure)
    {
        var builder = new PasswordStoreBuilder();
        var enhanced = configure(builder);
        var options = new PwXtOptions()
        {
            Salt = enhanced.Salt,
            Passphrase = enhanced.Passphrase
        };
        services.AddSingleton(options);
        services.AddSingleton(_ => enhanced.Build());
        services.AddSingleton<CommandFactory>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        
        return services;
    }
    
    /// <summary>
    /// The available store types
    /// </summary>
    public enum StoreType
    {
        LiteDb,
        Sqlite
    }
}