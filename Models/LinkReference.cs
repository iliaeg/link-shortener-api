using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LinkShortenerAPI.Models
{
    /// <summary>
    /// Describes the link reference in a database view.
    /// </summary>
    public class LinkReference
    {
        /// <summary>
        /// Gets or sets unique identifier used by MongoDb.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        [Required]
        public string OriginalLink { get; set; }

        [Required]
        public string ShortLink { get; set; }

        [Required]
        public ulong LinkIndex { get; set; }

        /// <summary>
        /// Gets or sets сounter of the number of clicks on the <see cref="ShortLink"/> link.
        /// </summary>
        public ulong ShortLinkClickCounter { get; set; } = 0;

        /// <summary>
        /// Gets or sets id of user who created this link.
        /// </summary>
        [Required]
        public ObjectId UserId { get; set; }
    }
}
