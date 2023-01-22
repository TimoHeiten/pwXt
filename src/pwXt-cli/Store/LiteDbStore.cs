using heitech.pwXtCli.Options;
using heitech.pwXtCli.ValueObjects;
using LiteDB;
using Microsoft.Extensions.Options;

namespace heitech.pwXtCli.Store
{
    /// <summary>
    /// <inheritdoc cref="IPasswordStore"/>
    /// </summary>
    public sealed class LiteDbStore : IPasswordStore, IDisposable
    {
        private readonly LiteDatabase _db;
        public LiteDbStore(IOptions<PwXtOptions> options)
        {
            _db = new LiteDatabase(options.Value.ConnectionString);
        }

        public Task AddPassword(Password password)
        {
            var db = GetCollection();

            var entity = new StoredPassword(ObjectId.NewObjectId(),  password.Key, password.Value, password.IV); 
            db.Insert(entity);
            db.EnsureIndex(x => x.Key);

            return Task.CompletedTask;
        }

        public Task DeletePassword(string key)
        {
            var db = GetCollection();
            db.DeleteMany(x => x.Key == key);
            return Task.CompletedTask;
        }

        public Task<Password> GetPassword(string key)
        {
            var db = GetCollection();
            var one = db.FindOne(x => x.Key == key);

            if (one is null)
                return Task.FromResult(Password.Empty);

            var pw = new Password(one.Key, one.Password, one.Vector);
            return Task.FromResult(pw);
        }

        public Task<IEnumerable<string>> ListKeys()
        {
            var db = GetCollection();
            var pws = db.FindAll().Select(x => x.Key);
            return Task.FromResult(pws);
        }

        public Task UpdatePassword(Password password)
        {
            var db = GetCollection();
            var one = db.FindOne(x => x.Key == password.Key);
            one.Password = password.Value;
            db.Update(one);

            return Task.CompletedTask;
        }
        private ILiteCollection<StoredPassword> GetCollection()
        {
            return _db.GetCollection<StoredPassword>("passwords");
        }

        public void Dispose()
            => _db.Dispose();
    }
}