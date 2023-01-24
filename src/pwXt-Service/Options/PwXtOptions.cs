namespace heitech.pwXtCli.Options
{
    ///<summary>
    /// Configuration for the CLI
    ///</summary>
    public sealed class PwXtOptions
    {
        ///<summary>
        /// the configured Passphrase
        ///</summary>
        public string Passphrase { get; set; } = default!;

        /// <summary>
        /// The Connection to the Stores database
        /// </summary>
        public string ConnectionString { get; set; } = default!;

        /// <summary>
        /// Salt for the encryption
        /// </summary>
        public string Salt { get; set; } = default!;
    }
}