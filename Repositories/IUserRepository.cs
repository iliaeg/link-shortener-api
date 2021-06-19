using System.Threading.Tasks;
using MongoDB.Bson;

using LinkShortenerAPI.Models;

namespace LinkShortenerAPI.Repositories
{
    /// <summary>
    /// Describes interface for Repository of the <see cref="User"/> objects.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Creates a new <see cref="User"/> instance in db.
        /// </summary>
        Task<ObjectId> Create(User user);

        /// <summary>
        /// Gets user by id.
        /// </summary>
        Task<User> Get(ObjectId objectId);

        /// <summary>
        /// Gets user by email.
        /// </summary>
        Task<User> Get(string email);
    }
}
