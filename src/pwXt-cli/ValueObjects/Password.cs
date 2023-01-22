namespace heitech.pwXtCli.ValueObjects
{
    ///<summary>
    /// Represents a password
    ///</summary>
    public readonly struct Password
    {
        /// <summary>
        /// The Id under which the password is stored
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The Encrypted password
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// The Vector used to encrypt the password
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string Key { get; } = default!;

        public Password(string id, string password, string key)
        {
            Id = id;
            Key = key;
            Value = password;
        }

        public static Password Empty => new Password(string.Empty, string.Empty, string.Empty);
        public bool IsEmpty => string.IsNullOrEmpty(Key) && string.IsNullOrEmpty(Value) && string.IsNullOrEmpty(Id);

        public override string ToString()
        {
            return $"Password: [{Key} : {Value}]";
        }
    }
}