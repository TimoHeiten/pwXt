// using heitech.pwXtCli.Options;
// using heitech.pwXtCli.ValueObjects;
// using Microsoft.Data.Sqlite;
// using Microsoft.Extensions.Options;
//
// namespace heitech.pwXtCli.Store;
//
// public sealed class SqliteStore : IPasswordStore
// {
//     private readonly string _connectionString;
//
//     public SqliteStore(IOptions<PwXtOptions> options)
//         => _connectionString = options.Value.ConnectionString;
//
//     public async Task AddPassword(Password password)
//     {
//         await using var connection = new SqliteConnection(_connectionString);
//         await connection.OpenAsync();
//
//         var command = connection.CreateCommand();
//         command.CommandText = "INSERT INTO Passwords (Key, Value, Vector) VALUES (@key, @value, @vector)";
//         command.Parameters.AddWithValue("@key", password.Key);
//         command.Parameters.AddWithValue("@value", password.Value);
//         command.Parameters.AddWithValue("@vector", password.IV);
//         
//         await command.ExecuteNonQueryAsync();
//     }
//
//     public async Task DeletePassword(string key)
//     {
//         await using var connection = new SqliteConnection(_connectionString);
//         await connection.OpenAsync();
//         var command = connection.CreateCommand();
//         command.CommandText = "DELETE FROM Passwords WHERE Key = @key";
//         command.Parameters.AddWithValue("@key", key);
//         await command.ExecuteNonQueryAsync();
//     }
//
//     public async Task<IEnumerable<string>> ListKeys()
//     {
//         await using var connection = new SqliteConnection(_connectionString);
//         await connection.OpenAsync();
//         var command = connection.CreateCommand();
//         command.CommandText = "SELECT Key FROM Passwords";
//
//         await using var reader = await command.ExecuteReaderAsync();
//         var keysList = new List<string>();
//         while (await reader.ReadAsync())
//         {
//             var value = reader["Value"].ToString();
//             keysList.Add(value!);
//         }
//         return keysList;
//     }
//
//     public async Task<Password> GetPassword(string key)
//     {
//         await using var connection = new SqliteConnection(_connectionString);
//         await connection.OpenAsync();
//         var command = connection.CreateCommand();
//
//         command.CommandText = "SELECT Value, Vector FROM Passwords WHERE Key = @key";
//         command.Parameters.AddWithValue("@key", key);
//         var reader = await command.ExecuteReaderAsync();
//         var password = "";
//         byte[] vector = default!;
//         if (!await reader.ReadAsync()) 
//             return Password.Empty;
//
//         password = Convert.ToString(reader["Value"]);
//         vector = (byte[])reader["Vector"];
//
//         return new Password(key, password!, vector);
//     }
//
//     public async Task UpdatePassword(Password password)
//     {
//         await using (var connection = new SqliteConnection(_connectionString))
//         {
//             await connection.OpenAsync();
//
//             var command = connection.CreateCommand();
//             command.CommandText = "UPDATE Passwords SET Password = @password WHERE Key = @key";
//             command.Parameters.AddWithValue("@key", password.Key);
//             command.Parameters.AddWithValue("@value", password.Value);
//             
//             await command.ExecuteNonQueryAsync();
//         }
//     }
// }