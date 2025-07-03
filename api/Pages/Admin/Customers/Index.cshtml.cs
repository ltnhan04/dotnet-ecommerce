using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using api.Dtos;
using api.models;
using Microsoft.EntityFrameworkCore;

namespace api.Pages.Admin.Customers
{
    public class Index : PageModel
    {
        private readonly ILogger<Index> _logger;
        private readonly iTribeDbContext _context;
        public List<User> Customers { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; } = 0;
        public string? Search { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public Index(ILogger<Index> logger, iTribeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task OnGetAsync(string? search, string? email, string? phone, int page = 1, int size = 10)
        {
            var query = _context.Users.Where(u => u.role == "user");
            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.name.Contains(search));
            if (!string.IsNullOrEmpty(email))
                query = query.Where(u => u.email.Contains(email));
            if (!string.IsNullOrEmpty(phone))
                query = query.Where(u => u.phoneNumber.Contains(phone));
            TotalCount = await query.CountAsync();
            Customers = await query.OrderByDescending(u => u.createdAt)
                                   .Skip((page - 1) * size)
                                   .Take(size)
                                   .ToListAsync();
            CurrentPage = page;
            PageSize = size;
            TotalPages = (int)Math.Ceiling((double)TotalCount / size);
            Search = search;
            Email = email;
            Phone = phone;
        }
    }
}