using UserService.Models;
using System.Collections.Generic;

namespace UserService.Data
{
    public static class UsersData
    {
        public static List<User> Users => new() {
            new User { Id = 1, Name = "Jhan" },
            new User { Id = 2, Name = "Jerone" },
        };
    }
}