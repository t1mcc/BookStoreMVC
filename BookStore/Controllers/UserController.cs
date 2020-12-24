using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        UserManager<User> _userManager;
        BookStoreDbContext _dbContext;

        public UserController(UserManager<User> userManager, BookStoreDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = await GetUserInfo(User);

            return View(model);
        }

        public IActionResult Orders() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var orders = _dbContext.Orders.Where(x => x.UserId == userId)
                                          .Include("BookOrders")
                                          .Include("BookOrders.Book")
                                          .ToList();

            return View(orders);
        }

        public async Task<IActionResult> ChangeProfile()
        {
            var model = await GetUserInfo(User);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfile(UserProfileViewModel model) 
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                user.UserName = model.UserName;
                user.PhoneNumber = model.Phone;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

            }

            return View();
        }

        private async Task<UserProfileViewModel> GetUserInfo(ClaimsPrincipal userClaimsPrincipal) {
            var user = await _userManager.GetUserAsync(userClaimsPrincipal);

            return new UserProfileViewModel
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        Phone = user.PhoneNumber
                    };
        }
    }
}
