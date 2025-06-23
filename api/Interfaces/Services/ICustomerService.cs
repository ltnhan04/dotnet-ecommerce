using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.models;

namespace api.Interfaces
{
    public interface ICustomerService
    {
        Task<User> FindCustomerById(string userId);
        Task<User> UpdateProfile(string userId, UpdateCustomerProfileDto dto);
        Task<User?> FindCustomerByEmail(string email);
        Task<User> RegisteredAccount(string email);
        Task<User> CreateNewCustomer(CreateUserDto dto);
        Task<User> VerifyRole(VerifyRoleDto dto);

    }
}