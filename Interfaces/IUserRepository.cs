using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pruebaidwm.Models;

namespace pruebaidwm.Interfaces
{
    public class IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<bool> UserExistsByRut(string rut);
        Task<bool> SaveChangesAsync();
    }
}