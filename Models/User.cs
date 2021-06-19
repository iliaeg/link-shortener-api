using MongoDB.Bson;

namespace LinkShortenerAPI.Models
{
    /// <summary>
    /// Describes the user in a database view.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier used by MongoDb.
        /// </summary>
        public ObjectId Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
