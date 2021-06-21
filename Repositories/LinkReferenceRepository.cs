using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

using LinkShortenerAPI.Models;
using System.Collections.Generic;

namespace LinkShortenerAPI.Repositories
{
    /// <summary>
    /// Implements interface for Repository of the <see cref="LinkReference"/> objects.
    /// </summary>
    public class LinkReferenceRepository : ILinkReferenceRepository
    {
        private readonly IMongoCollection<LinkReference> linkRefs;

        public LinkReferenceRepository(IMongoClient client, DatabaseSettings databaseSettings)
        {
            var database = client.GetDatabase(databaseSettings.LinksDatabaseName);
            var collection = database.GetCollection<LinkReference>(nameof(LinkReference));
            linkRefs = collection;
        }

        /// <inheritdoc/>
        public async Task<ObjectId> Create(LinkReference linkRef)
        {
            await linkRefs.InsertOneAsync(linkRef);
            return linkRef.Id;
        }

        /// <inheritdoc/>
        public Task<LinkReference> GetByOriginal(string originalLink)
        {
            var filter = Builders<LinkReference>.Filter.Eq(l => l.OriginalLink, originalLink);
            var linkRef = linkRefs.Find(filter).FirstOrDefaultAsync();
            return linkRef;
        }

        /// <inheritdoc/>
        public Task<LinkReference> GetByShort(string shortLink)
        {
            var filter = Builders<LinkReference>.Filter.Eq(l => l.ShortLink, shortLink);
            var linkRef = linkRefs.Find(filter).FirstOrDefaultAsync();
            return linkRef;
        }

        /// <inheritdoc/>
        public ulong? GetLastIndex()
        {
            var linkRef = linkRefs.Find(x => true).SortByDescending(l => l.LinkIndex).Limit(1).FirstOrDefaultAsync().Result;

            // if link ref is null, no link refs present in db.
            if (linkRef is null)
            {
                return 0;
            }

            return linkRef.LinkIndex;
        }

        /// <inheritdoc/>
        public void IncrementShortLinkCounter(ObjectId objectId)
        {
            var filter = Builders<LinkReference>.Filter.Eq(l => l.Id, objectId);
            var update = Builders<LinkReference>.Update.Inc("ShortLinkClickCounter", 1);

            // atomic operation
            linkRefs.FindOneAndUpdate(filter, update);
        }

        /// <inheritdoc/>
        public Task<List<LinkReference>> GetLinks(ObjectId userId, int? limit = null, int? skip = null)
        {
            var filter = Builders<LinkReference>.Filter.Eq(l => l.UserId, userId);
            var links = linkRefs.Find(filter).Skip(skip).Limit(limit).ToListAsync();
            return links;
        }
    }
}
