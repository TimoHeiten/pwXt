using System.Security.Cryptography;
using System.Text;
using heitech.pwXtCli.Options;

namespace heitech.pwXtCli.ValueObjects;

public static class PasswordExtensions
{
    public static string Convert(this byte[] b) => Encoding.Unicode.GetString(b);
    public static byte[] Convert(this string s) => Encoding.Unicode.GetBytes(s);

    /// <summary>
    /// Encrypt the desired pw and return the encrypted pw object
    /// </summary>
    /// <param name="options"></param>
    /// <param name="id"></param>
    /// <param name="password"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static async Task<Password> EncryptAsync(this PwXtOptions options, string id, string password, byte[]? iv = null)
    {
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;

        byte[] vector;
        if (iv is null)
        {
            aes.GenerateIV();
            vector = aes.IV;
        }
        else
        {
            vector = iv;
        }
        var key = DeriveKeyFromPassphrase(options);

        await using MemoryStream output = new();
        await using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(key, vector), CryptoStreamMode.Write);
        await cryptoStream.WriteAsync(password.Convert());
        await cryptoStream.FlushFinalBlockAsync();

        var pw = output.ToArray().Convert();
        return new Password(id, pw, vector);
    }

    /// <summary>
    /// Decrypt the password and return the decrypted password value
    /// </summary>
    /// <param name="options"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static async Task<string> DecryptAsync(this PwXtOptions options, Password password)
    {
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;
        var key = DeriveKeyFromPassphrase(options);

        await using MemoryStream input = new(password.Value.Convert());
        await using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(key, password.IV), CryptoStreamMode.Read);
        await using MemoryStream output = new();

        await cryptoStream.CopyToAsync(output);

        return output.ToArray().Convert();
    }

    private static byte[] DeriveKeyFromPassphrase(PwXtOptions options)
    {
        var salting = options.Salt.Convert();
        const int iterations = 1000;
        const int desiredKeyLength = 16; // 16 bytes equal 128 bits.
        var hashMethod = HashAlgorithmName.SHA384;
        return Rfc2898DeriveBytes.Pbkdf2(options.Passphrase.Convert(),
            salting,
            iterations,
            hashMethod,
            desiredKeyLength);
    }
}