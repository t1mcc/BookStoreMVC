using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookStore.Repositories
{
    public class EFOrderRepository : EFRepositoryBase<Order>, IOrderRepository
    {
        public EFOrderRepository(BookStoreDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<Order>> GetUserOrders(string userId)
        {
            return await _dbContext.Orders.Where(x => x.UserId == userId)
                                           .Include("BookOrders")
                                           .Include("BookOrders.Book")
                                           .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrders(Expression<Func<Order, bool>> @where)
        {
            return await _dbContext.Orders.Where(where)
                                        .Include("User")
                                        .Include("BookOrders")
                                        .Include("BookOrders.Book")
                                        .ToListAsync();
                              
        }

    }
}
