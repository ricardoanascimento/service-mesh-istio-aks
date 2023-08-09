
using System.Text.Json.Serialization;

namespace UserService.Models
{
    public class Vote
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("votedItemId")]
        public int VotedItemId { get; set; }
    }
}