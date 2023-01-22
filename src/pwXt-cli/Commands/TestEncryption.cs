// using System.Diagnostics;
// using CliFx;
// using CliFx.Attributes;
// using CliFx.Infrastructure;
// using CryptoNet;
// using heitech.pwXtCli.Options;
// using heitech.pwXtCli.ValueObjects;
//
// namespace heitech.pwXtCli.Commands;
//
// [Command("test", Description = "test the encryption on a a password")]
// public class TestEncryption : ICommand
// {
//     public ValueTask ExecuteAsync(IConsole console)
//     {
//         var options = new PwXtOptions
//         {
//             Passphrase = "abcaffeschnee-abcaffeschneeabcaffeschneeabcaffeschneeabcaffeschneeabcaffeschnee",
//             ConnectionString = "does not matter"
//         };
//
//         ICryptoNet cryptoNet = new CryptoNetAes();
//         var key = cryptoNet.ExportKey();
//
//         ICryptoNet encryptClient = new CryptoNetAes(key);
//         var encrypt = encryptClient.EncryptFromString(options.Passphrase);
//
//         ICryptoNet decryptClient = new CryptoNetAes(key);
//         var decrypt = decryptClient.DecryptToString(encrypt);
//
//         Debug.Assert(options.Passphrase == decrypt);
//         Console.WriteLine(options.Passphrase);
//         Console.WriteLine(decrypt);
//
//         return ValueTask.CompletedTask;
//     }
// }