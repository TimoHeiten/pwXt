using heitech.pwXtCli.ValueObjects;

namespace heitech.pwXtCli.Store
{
    ///<summary>
    /// The interface for the password store
    ///</summary>
    public interface IPasswordStore
    {
        ///<summary>
        /// Get a password from the store
        ///</summary>
        Task<Password> GetPasswordAsync(string key);

        ///<summary>
        /// Add a password to the store
        ///</summary>
        Task AddPasswordAsync(Password password);

        ///<summary>
        /// Update a password in the store
        ///</summary>
        Task UpdatePasswordAsync(Password password);

        ///<summary>
        /// (hard) Delete a password from the store
        ///</summary>
        Task DeletePasswordAsync(string key);
        
        ///<summary>
        /// Returns all Keys in the store
        ///</summary>
        Task<IEnumerable<string>> ListKeysAsync();
    }
}