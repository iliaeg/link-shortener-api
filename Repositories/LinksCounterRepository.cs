using MongoDB.Driver;

using LinkShortenerAPI.Models;

namespace LinkShortenerAPI.Repositories
{
    /// <summary>
    /// Implements interface for Repository with the <see cref="LinksCounter"/> object.
    /// </summary>
    public class LinksCounterRepository : ILinksCounterRepository
    {
        private readonly IMongoCollection<LinksCounter> linksCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinksCounterRepository"/> class.
        /// </summary>
        public LinksCounterRepository(IMongoClient client, DatabaseSettings databaseSettings)
        {
            var database = client.GetDatabase(databaseSettings.LinksDatabaseName);
            var collection = database.GetCollection<LinksCounter>(nameof(LinksCounter));
            linksCounter = collection;
        }

        /// <inheritdoc/>
        public LinksCounter IncrementCounter()
        {
            var filter = Builders<LinksCounter>.Filter.Eq(c => c.Id, "Id");
            var update = Builders<LinksCounter>.Update.Inc("Value", 1);

            // atomic operation
            var counter = linksCounter.FindOneAndUpdate(
                filter,
                update,
                new FindOneAndUpdateOptions<LinksCounter>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After,
                }
            );
            return counter;
        }
    }
}
