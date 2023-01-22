namespace heitech.pwXtCli.ValueObjects
{
    ///<summary>
    /// Represents a password
    ///</summary>
    public readonly struct Password
    {
        /// <summary>
        /// The Key under which the password is stored
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// the Encrypted password
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// The Vector for the encryption
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public byte[] IV { get; } = default!;

        public Password(string key, string password, byte[] iv)
        {
            IV = iv;
            Key = key;
            Value = password;
        }

        public static Password Empty => new Password(string.Empty, string.Empty, Array.Empty<byte>());
        public bool IsEmpty => string.IsNullOrEmpty(Key) && string.IsNullOrEmpty(Value);

        public override string ToString()
        {
            return $"Password: [{Key} : {Value}]";
        }
    }
}