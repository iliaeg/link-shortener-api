using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

using LinkShortenerAPI.Models;

namespace LinkShortenerAPI.Repositories
{
    /// <summary>
    /// Describes interface for Repository of the <see cref="LinkReference"/> objects.
    /// </summary>
    public interface ILinkReferenceRepository
    {
        /// <summary>
        /// Creates a new <see cref="LinkReference"/> instance in db.
        /// </summary>
        Task<ObjectId> Create(LinkReference link);

        /// <summary>
        /// Gets a link ref by the original link.
        /// </summary>
        Task<LinkReference> GetByOriginal(string originalLink);

        /// <summary>
        /// Gets a link ref by the short link.
        /// </summary>
        Task<LinkReference> GetByShort(string shortLink);

        /// <summary>
        /// Gets last link index in system.
        /// </summary>
        ulong? GetLastIndex();

        /// <summary>
        /// Increases short link counter.
        /// </summary>
        void IncreaseShortLinkCounter(ObjectId objectId);

        /// <summary>
        /// Gets links created by specific user.
        /// </summary>
        Task<List<LinkReference>> GetLinks(ObjectId userId, int? limit = null, int? skip = null);
    }
}
