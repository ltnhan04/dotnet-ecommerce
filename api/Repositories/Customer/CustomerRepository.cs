using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories.Customer
{
    public class CustomerRepository
    {
        private readonly iTribeDbContext _context;
        public CustomerRepository(iTribeDbContext context)
        {
            _context = context;
        }
        public async Task<User?> FindById(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User?> FindByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.email == email);
        }
        public async Task<User> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User> UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }


    }
}