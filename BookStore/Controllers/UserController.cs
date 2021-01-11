using System.Security.Claims;
using System.Threading.Tasks;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IOrderRepository _orderRepository;

        public UserController(UserManager<User> userManager, IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = new UserProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber
            };

            return View(model);
        }

        public async Task<IActionResult> Orders() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var orders = await _orderRepository.GetUserOrders(userId);

            return View(orders);
        }

        public async Task<IActionResult> ChangeProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = new UserProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber
            };

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

    }
}
