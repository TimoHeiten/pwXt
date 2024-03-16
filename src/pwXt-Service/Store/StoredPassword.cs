using LiteDB;

namespace heitech.pwXtCli.Store;

/// <summary>
/// The class that maps to the BsonDocument for our store 
/// </summary>
public sealed class StoredPassword
{
    public ObjectId Id { get; set; }
    public string Key { get; set; }
    public string PasswordId { get; }
    public string Password { get; set; }

    public StoredPassword(ObjectId id, string passwordId, string password, string key)
    {
        Id = id;
        Key = key;
        Password = password;
        PasswordId = passwordId;
    }

}