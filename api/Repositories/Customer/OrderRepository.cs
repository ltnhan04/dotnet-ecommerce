using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.models;

namespace api.Repositories.Customer
{
    public class OrderRepository: IOrderRepository
    {
        private readonly iTribeDbContext _context;
        public OrderRepository(iTribeDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

    }
}