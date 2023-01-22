using System.Text;
using CryptoNet;
using heitech.pwXtCli.Store;

namespace heitech.pwXtCli.ValueObjects;

public static class PasswordExtensions
{
    private static string Convert(this byte[] b) => Encoding.Unicode.GetString(b);
    private static byte[] Convert(this string s) => Encoding.Unicode.GetBytes(s);

    /// <summary>
    /// Encrypt the desired pw and return the encrypted pw object
    /// </summary>
    /// <param name="_">just for extension method availability</param>
    /// <param name="id"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static Password Encrypt(this IPasswordStore _, string id, string password)
    {
        var cryptoNet = new CryptoNetAes();
        var key = cryptoNet.ExportKey();

        var encryptClient = new CryptoNetAes(key);
        var encrypted = encryptClient.EncryptFromString(password);

        return new Password(id, encrypted.Convert(), key);
    }

    /// <summary>
    /// Decrypt the password and return the decrypted password value
    /// </summary>
    /// <returns></returns>
    public static string Decrypt(this Password password)
    {
        var decryptClient = new CryptoNetAes(password.Key);
        var result = decryptClient.DecryptToString(password.Value.Convert());
        return result;
    }
}