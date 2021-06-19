using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

using LinkShortenerAPI.Models;

namespace LinkShortenerAPI.Repositories
{
    /// <summary>
    /// Implements interface for Repository of the <see cref="User"/> objects.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> users;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        public UserRepository(IMongoClient client, DatabaseSettings databaseSettings)
        {
            var database = client.GetDatabase(databaseSettings.UsersDatabaseName);
            var collection = database.GetCollection<User>(nameof(User));
            users = collection;
        }

        /// <inheritdoc/>
        public async Task<ObjectId> Create(User user)
        {
            await users.InsertOneAsync(user);
            return user.Id;
        }

        /// <inheritdoc/>
        public Task<User> Get(ObjectId objectId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, objectId);
            var user = users.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        /// <summary>
        /// Describes interface for Repository of the <see cref="User"/> objects.
        /// </summary>
        /// <remarks>If users more than one, returns first of them.</remarks>
        public Task<User> GetByEmail(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var user = users.Find(filter).FirstOrDefaultAsync();
            return user;
        }
    }
}
