using VotingService.Models;
using System.Collections.Generic;

namespace VotingService.Data
{
    public static class VotingData
    {
        public static List<Vote> Votes => new List<Vote>() {
            new Vote { Id = 1, UserId  = 1, VotedItemId  = 1 },
            new Vote { Id = 2, UserId  = 2, VotedItemId  = 1 },
        };
    }
}