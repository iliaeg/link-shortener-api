using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LinkShortenerAPI.Models
{
    /// <summary>
    /// Describes the link in a database view.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Gets or sets unique identifier used by MongoDb.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        public string OriginalLink { get; set; }

        public string ShortLink { get; set; }

        /// <summary>
        /// Gets or sets сounter of the number of clicks on the <see cref="ShortLink"/> link.
        /// </summary>
        public ulong ShortLinkCounter { get; set; }

        /// <summary>
        /// Gets or sets id of user who created this link.
        /// </summary>
        public ObjectId UserId { get; set; }
    }
}
