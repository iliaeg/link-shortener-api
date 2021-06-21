using MongoDB.Bson.Serialization.Attributes;

namespace LinkShortenerAPI.Models
{
    /// <summary>
    /// Describes the link counter in a database view.
    /// </summary>
    public class LinksCounter
    {
        /// <summary>
        /// Unique identifier used by MongoDb.
        /// </summary>
        [BsonId]
        public string Id { get; set; } = "Id";

        public ulong Value { get; set; } = 0;
    }
}