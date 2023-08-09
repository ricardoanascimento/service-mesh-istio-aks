using System.Net.Http;
using System.Text.Json;
using UserService.Models;

namespace UserService.Services
{
    public class VotingService : IVotingService
    {
        private readonly HttpClient _httpClient;

        public VotingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Vote>> GetVoteByUserId(int userId)
        {
            // Use the service name as the hostname for service discovery
            var votingServiceUrl = "http://voting-service.default.svc.cluster.local:8081";

            var response = await _httpClient.GetAsync($"{votingServiceUrl}/{userId}");
            // Process the response and return the result

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Deserialize the response content into a List<Vote> instance
                List<Vote>? votes = JsonSerializer.Deserialize<List<Vote>?>(content);

                return votes ?? new List<Vote>();
            }
            else
                return new List<Vote>();
        }
    }
}