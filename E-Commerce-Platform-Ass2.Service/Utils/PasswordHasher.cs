using System;
using BCrypt.Net;

namespace E_Commerce_Platform_Ass2.Service.Utils
{
    /// <summary>
    /// Utility class for password hashing and verification using BCrypt
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Hashes a password using BCrypt algorithm
        /// BCrypt automatically generates and stores salt with the hash
        /// </summary>
        /// <param name="password">Plain text password to hash</param>
        /// <returns>Hashed password string</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        /// <param name="password">Plain text password to verify</param>
        /// <param name="hash">Hashed password to compare against</param>
        /// <returns>True if password matches the hash, false otherwise</returns>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                // Invalid hash format or other BCrypt errors
                return false;
            }
        }

        /// <summary>
        /// Checks if a string is a valid BCrypt hash
        /// BCrypt hashes typically start with $2a$, $2b$, or $2y$
        /// </summary>
        /// <param name="hash">String to check</param>
        /// <returns>True if the string appears to be a BCrypt hash</returns>
        public static bool IsBcryptHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            // BCrypt hashes start with $2a$, $2b$, $2x$, or $2y$ followed by cost parameter
            return hash.StartsWith("$2a$") || 
                   hash.StartsWith("$2b$") || 
                   hash.StartsWith("$2x$") || 
                   hash.StartsWith("$2y$");
        }

        /// <summary>
        /// Verifies password with backward compatibility support
        /// Supports both BCrypt hashed passwords and plain text passwords (for migration)
        /// </summary>
        /// <param name="password">Plain text password to verify</param>
        /// <param name="storedPassword">Stored password (can be hash or plain text)</param>
        /// <returns>True if password matches, false otherwise</returns>
        public static bool VerifyPasswordWithBackwardCompat(string password, string storedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedPassword))
            {
                return false;
            }

            // If stored password is a BCrypt hash, use normal verification
            if (IsBcryptHash(storedPassword))
            {
                return VerifyPassword(password, storedPassword);
            }

            // Otherwise, compare as plain text (for backward compatibility during migration)
            return password.Equals(storedPassword, StringComparison.Ordinal);
        }
    }
}
