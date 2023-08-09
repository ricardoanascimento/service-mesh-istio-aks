using UserService.Models;

namespace UserService.Services
{
    public interface IVotingService
    {
        Task<List<Vote>> GetVoteByUserId(int userId);
    }
}