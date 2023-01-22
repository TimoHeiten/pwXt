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

            var entity = new StoredPassword(ObjectId.NewObjectId(),  password.Id, password.Value, password.Key); 
            db.Insert(entity);
            db.EnsureIndex(x => x.PasswordId);

            return Task.CompletedTask;
        }

        public Task DeletePassword(string id)
        {
            var db = GetCollection();
            db.DeleteMany(x => x.PasswordId == id);
            return Task.CompletedTask;
        }

        public Task<Password> GetPassword(string id)
        {
            var db = GetCollection();
            var one = db.FindOne(x => x.PasswordId == id);

            if (one is null)
                return Task.FromResult(Password.Empty);

            var pw = new Password(one.PasswordId, one.Password, one.Key);
            return Task.FromResult(pw);
        }

        public Task<IEnumerable<string>> ListKeys()
        {
            var db = GetCollection();
            var pws = db.FindAll().Select(x => x.PasswordId);
            return Task.FromResult(pws);
        }

        public Task UpdatePassword(Password password)
        {
            var db = GetCollection();
            var one = db.FindOne(x => x.PasswordId == password.Id);

            one.Password = password.Value;
            one.Key = password.Key;
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