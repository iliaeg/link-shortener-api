﻿using System.ComponentModel.DataAnnotations;
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

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
