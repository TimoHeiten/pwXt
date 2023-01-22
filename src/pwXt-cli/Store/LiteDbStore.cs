using heitech.pwXtCli.Options;
using heitech.pwXtCli.ValueObjects;
using LiteDB;
using Microsoft.Extensions.Options;

namespace heitech.pwXtCli.Store
{
    /// <summary>
    /// <inheritdoc cref="IPasswordStore"/>
    /// </summary>
    public sealed class LiteDbStore : IPasswordStore
    {
        private readonly PwXtOptions _options;

        public LiteDbStore(IOptions<PwXtOptions> options)
            => _options = options.Value;

        public Task AddPasswordAsync(Password password)
        {
            var db = GetCollection();

            var entity = new StoredPassword(ObjectId.NewObjectId(),  password.Key, password.Value, password.IV); 
            db.Insert(entity);
            db.EnsureIndex(x => x.Key);

            return Task.CompletedTask;
        }

        public Task DeletePasswordAsync(string key)
        {
            var db = GetCollection();
            db.DeleteMany(x => x.Key == key);
            return Task.CompletedTask;
        }

        public Task<Password> GetPasswordAsync(string key)
        {
            var db = GetCollection();
            var one = db.FindOne(x => x.Key == key);

            if (one is null)
                return Task.FromResult(Password.Empty);

            var pw = new Password(one.Key, one.Password, one.Vector);
            return Task.FromResult(pw);
        }

        public Task<IEnumerable<string>> ListKeysAsync()
        {
            var db = GetCollection();
            var pws = db.FindAll().Select(x => x.Key);
            return Task.FromResult(pws);
        }

        public Task UpdatePasswordAsync(Password password)
        {
            var db = GetCollection();
            var one = db.FindOne(x => x.Key == password.Key);
            one.Password = password.Value;
            db.Update(one);

            return Task.CompletedTask;
        }
        private ILiteCollection<StoredPassword> GetCollection()
        {
            var db = new LiteDatabase(_options.ConnectionString);
            return db.GetCollection<StoredPassword>("passwords");
        }

        private sealed class StoredPassword
        {
            public ObjectId Id { get; set; }
            public string Key { get; }
            public byte[] Vector { get; }
            public string Password { get; set; }

            public StoredPassword(ObjectId id, string key, string password, byte[] vector)
            {
                Id = id;
                Key = key;
                Vector = vector;
                Password = password;
            }

        }
    }
}