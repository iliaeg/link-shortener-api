using System;
using Microsoft.AspNetCore.WebUtilities;

namespace LinkShortenerAPI.Helpers
{
    /// <summary>
    /// Аllows to make short links by original.
    /// </summary>
    /// <remarks>Max short links count in system is 10^19,
    /// because of the shortener algorithm.</remarks>
    public static class LinkShortener
    {
        /// <summary>
        /// Creates new short link.
        /// </summary>
        /// <param name="index">Index of the current link.</param>
        /// <returns>Shortened link.</returns>
        public static string CreateShortLink(ulong index)
        {
            return WebEncoders.Base64UrlEncode(BitConverter.GetBytes(index));
        }
    }
}
