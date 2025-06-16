using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces;
using api.models;
using api.Repositories;
using api.Repositories.Customer;
using api.Utils;
using BCrypt.Net;

namespace api.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly RedisRepository _redis;
        private readonly CustomerRepository _customerRepository;
        public CustomerService(RedisRepository redis, CustomerRepository customerRepository)
        {
            _redis = redis;
            _customerRepository = customerRepository;
        }
        public async Task<User> FindCustomerById(string userId)
        {
            var customer = await _customerRepository.FindById(userId);
            return customer ?? throw new AppException("Customer not found", 404);
        }
        public async Task<User> UpdateProfile(string userId, UpdateCustomerProfileDto dto)
        {
            var customer = await _customerRepository.FindById(userId);
            if (customer == null) throw new AppException("Customer not found", 404);
            customer.name = dto.name;
            customer.phoneNumber = dto.phoneNumber;
            customer.address = dto.address;
            customer.updatedAt = DateTime.UtcNow;
            return await _customerRepository.UpdateUser(customer) ?? throw new AppException("Update failed", 500);
        }
        public async Task<User?> FindCustomerByEmail(string email)
        {
            var customer = await _customerRepository.FindByEmail(email);
            if (customer != null) throw new AppException("Customer already exists", 400);
            return null;
        }
        public async Task<User> RegisteredAccount(string email)
        {
            var user = await _customerRepository.FindByEmail(email);
            return user ?? throw new AppException("Customer not found", 404);
        }
        public async Task<User> CreateNewCustomer(CreateUserDto dto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);
            var customer = new User
            {
                name = dto.name,
                email = dto.email,
                password = hashedPassword,
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow,
            };
            var result = await _customerRepository.CreateUser(customer);
            await _redis.DeleteAsync($"signup:{dto.email}");
            return result;
        }
        public async Task<User> VerifyRole(VerifyRoleDto dto)
        {
            var customer = await _customerRepository.FindByEmail(dto.email);
            if (customer == null || customer.password != dto.password)
                throw new AppException("Invalid credentials", 401);
            if (customer.role.ToString() != dto.role.ToString())
                throw new AppException("Access denied - Admin only", 403);
            return customer;
        }
    }
}