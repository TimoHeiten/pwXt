using heitech.pwXtCli.Store;
using Microsoft.Extensions.DependencyInjection;

namespace pwXt_Service.Store;

public sealed class PasswordStoreBuilder
{
    // defaults are liteDb
    private string ConnectionString { get; set; }
    internal string Salt { get; set; }
    internal string Passphrase { get; set; }
    
    private Dependencies.StoreType StoreType { get; set; }

    public PasswordStoreBuilder()
    {
        var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var defaultPath = Path.Combine(userPath, "pw-store.db");
        ConnectionString = $"Filename={defaultPath}; Connection=Shared;";

        Salt = "not-so-secret-salt";
        Passphrase = "not-so-secret-passphrase";
        StoreType = Dependencies.StoreType.LiteDb;
    }
    
    public PasswordStoreBuilder WithStoreType(Dependencies.StoreType storeType)
    {
        StoreType = storeType;
        return this;
    }

    public PasswordStoreBuilder WithConnectionString(string connectionString)
    {
        ConnectionString = connectionString;
        return this;
    }
    
    public PasswordStoreBuilder WithSalt(string salt)
    {
        Salt = salt;
        return this;
    }
    
    public PasswordStoreBuilder WithPassphrase(string passphrase)
    {
        Passphrase = passphrase;
        return this;
    }
    
    internal IPasswordStore Build()
    {
        return StoreType switch
        {
            Dependencies.StoreType.LiteDb => new LiteDbStore(ConnectionString),
            Dependencies.StoreType.Sqlite => throw new NotImplementedException("currently not working with sqlite"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}