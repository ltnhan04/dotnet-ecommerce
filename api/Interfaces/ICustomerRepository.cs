using System.Threading.Tasks;
using api.models;

namespace api.Interfaces
{
    public interface ICustomerRepository
    {
        Task<User?> FindById(string userId);
        Task<User?> FindByEmail(string email);
        Task<User> CreateUser(User user);
        Task<User> UpdateUser(User user);
    }
} 