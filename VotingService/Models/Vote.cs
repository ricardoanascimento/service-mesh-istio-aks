
namespace VotingService.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VotedItemId { get; set; }
    }
}