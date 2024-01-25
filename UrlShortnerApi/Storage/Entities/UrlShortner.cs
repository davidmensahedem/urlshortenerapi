using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortnerApi.Storage.Entities
{
    public class UrlShortner
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string? LongUrl { get; set; }
        public string? ShortUrl { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
