using System.ComponentModel.DataAnnotations;

namespace LinkShortenerAPI.Models
{
    public class ShortLinkRequest
    {
        [Required]
        public string Url { get; set; }
    }
}
