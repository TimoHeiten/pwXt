using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using CryptoNet;
using CryptoNet.Models;
using heitech.pwXtCli.Options;

namespace heitech.pwXtCli.ValueObjects;

public static class PasswordExtensions
{
    // todo create for each key , instead of using this one for all of them
    private static byte[] VectorBytes =
    {
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };
    private static string Convert(this byte[] b) => Encoding.Unicode.GetString(b);
    private static byte[] Convert(this string s) => Encoding.Unicode.GetBytes(s);

    /// <summary>
    /// Encrypt the desired pw and return the encrypted pw object
    /// </summary>
    /// <param name="_">just for extension method availability</param>
    /// <param name="options"></param>
    /// <param name="id"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static Password Encrypt(this PwXtOptions options, string id, string password)
    {
        var key = AsXmlKey(DeriveKeyFromPassword(options));
        var encryptClient = new CryptoNetAes(key);
        var encrypted = encryptClient.EncryptFromString(password);

        return new Password(id, encrypted.Convert(), key);
    }

    private static string AsXmlKey(byte[] key)
    {
        var aesKeyValue = new AesKeyValue
        {
            Key = key,
            Iv = VectorBytes
        };
        var serializer = new XmlSerializer(typeof(AesKeyValue));
        var writer = new StringWriter();
        serializer.Serialize(writer, aesKeyValue);
        writer.Close();
        return writer.ToString();
    }

    /// <summary>
    /// Decrypt the password and return the decrypted password value
    /// </summary>
    /// <returns></returns>
    public static string Decrypt(this PwXtOptions options, Password password)
    {
        var decryptClient = new CryptoNetAes(AsXmlKey(DeriveKeyFromPassword(options)));
        var result = decryptClient.DecryptToString(password.Value.Convert());

        return result;
    }
    
    private static byte[] DeriveKeyFromPassword(PwXtOptions options)
    {
        var emptySalt = options.Salt.Convert();
        var iterations = 1000;
        var desiredKeyLength = 32; // 16 bytes equal 256 bits.
        var hashMethod = HashAlgorithmName.SHA384;
        return Rfc2898DeriveBytes.Pbkdf2(options.Passphrase.Convert(),
            emptySalt,
            iterations,
            hashMethod,
            desiredKeyLength);
    }
}