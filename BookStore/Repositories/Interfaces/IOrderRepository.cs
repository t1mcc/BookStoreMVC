using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookStore.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetUserOrders(string userId);
        Task<IEnumerable<Order>> GetOrders(Expression<Func<Order, bool>> @where);
    }
}
