namespace LinkShortenerAPI.Models
{
    /// <summary>
    /// Stores settings for establishing a connection to the database.
    /// </summary>
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }

        public string UsersDatabaseName { get; set; }

        public string LinksDatabaseName { get; set; }
    }
}
