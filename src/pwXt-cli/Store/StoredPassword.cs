using LiteDB;

namespace heitech.pwXtCli.Store;

/// <summary>
/// The class that maps to the BsonDocument for our store 
/// </summary>
public sealed class StoredPassword
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