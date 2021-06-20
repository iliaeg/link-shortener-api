using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

using LinkShortenerAPI.Models;

namespace LinkShortenerAPI.Repositories
{
    /// <summary>
    /// Describes interface for Repository of the <see cref="Link"/> objects.
    /// </summary>
    public interface ILinkRepository
    {
        /// <summary>
        /// Gets (or creates) a short link from the original.
        /// </summary>
        Task<string> GetShortLink(string originalLink);

        /// <summary>
        /// Gets an original link by short.
        /// And increases the <see cref="Link.ShortLinkCounter"/> click counter.
        /// </summary>
        Task<string> GetOriginalLink(string shortLink);

        /// <summary>
        /// Gets links created by specific user.
        /// </summary>
        Task<IEnumerable<Link>> GetLinks(ObjectId userId);
    }
}
