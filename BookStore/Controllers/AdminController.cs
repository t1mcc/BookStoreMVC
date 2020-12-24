using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        UserManager<User> _userManager;
        BookStoreDbContext _dbContext;

        public AdminController(UserManager<User> userManager, BookStoreDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList()
            .Select(x => new UserProfileViewModel
            {
                UserName = x.UserName,
                Email = x.Email,
                Phone = x.PhoneNumber,
                Role = _userManager.GetRolesAsync(x).Result[0]
            });

            return View(users);
        }

        public IActionResult ActiveOrders()
        {
            var orders = GetOrders(false);

            return View(orders);
        }

        public IActionResult CompletedOrders() 
        {
            var orders = GetOrders(true);

            return View(orders);
        }

        private IEnumerable<Order> GetOrders(bool onlyCompletedOrders)
        {
            return _dbContext.Orders.Where(x => (x.OrderFulfilled == null) != onlyCompletedOrders)
                              .Include("User")
                              .Include("BookOrders")
                              .Include("BookOrders.Book")
                              .ToList();
        }
    }
}
