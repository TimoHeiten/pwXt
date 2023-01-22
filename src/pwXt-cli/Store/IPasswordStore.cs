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
        Task<Password> GetPassword(string id);

        ///<summary>
        /// Add a password to the store
        ///</summary>
        Task AddPassword(Password password);

        ///<summary>
        /// Update a password in the store
        ///</summary>
        Task UpdatePassword(Password password);

        ///<summary>
        /// (hard) Delete a password from the store
        ///</summary>
        Task DeletePassword(string key);
        
        ///<summary>
        /// Returns all Keys in the store
        ///</summary>
        Task<IEnumerable<string>> ListKeys();
    }
}