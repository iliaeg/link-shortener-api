using System;
using Microsoft.AspNetCore.WebUtilities;

namespace LinkShortenerAPI.Helpers
{
    /// <summary>
    /// Аllows to make short links by original and otherwise.
    /// </summary>
    /// <remarks>Max short links count in system is 10^19,
    /// because of the shortener algorithm.</remarks>
    public static class LinkShortener
    {
        /// <summary>
        /// Index of the link in system.
        /// </summary>
        public static ulong? Index { get; set; }

        /// <summary>
        /// Creates new short link.
        /// </summary>
        /// <returns>Shortened link.</returns>
        public static string CreateShortLink()
        {
            if (!Index.HasValue)
            {
                throw new NullReferenceException(nameof(Index));
            }

            Index++;
            return WebEncoders.Base64UrlEncode(BitConverter.GetBytes(Index.Value));
        }
    }
}
