using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IBookRepository _bookRepository;

        public AdminController(UserManager<User> userManager, IOrderRepository orderRepository, 
                               IBookRepository bookRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _bookRepository = bookRepository;
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

        public async Task<IActionResult> ActiveOrders()
        {
            var orders = await _orderRepository.GetOrders(x => x.OrderFulfilled == null);

            return View(orders);
        }

        public async Task<IActionResult> CompletedOrders() 
        {
            var orders = await _orderRepository.GetOrders(x => x.OrderFulfilled != null);

            return View(orders);
        }
        
        public IActionResult AddUser() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(RegisterViewModel model, string role) {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View();
        }

        public async Task<IActionResult> Books()
        {
            var books = await _bookRepository.GetAll();
            return View(books);
        }

        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", model.Photo.FileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }

                var book = new Book
                {
                    Name = model.Name,
                    Author = model.Author,
                    Category = model.Category,
                    Description = model.Description,
                    Price = model.Price,
                    Photo = model.Photo.FileName
                };

                await _bookRepository.Add(book);
            }

            return View();
        }

        public async Task<IActionResult> EditBook(int bookId) 
        {
            ViewBag.bookId = bookId;
            var book = await _bookRepository.GetById(bookId);
            
            var model = new BookViewModel
            {
                Name = book.Name,
                Author = book.Author,
                Category = book.Category,
                Description = book.Description,
                Price = book.Price
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(BookViewModel model, int bookId)
        {
            if (ModelState.IsValid)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", model.Photo.FileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }

                var book = await _bookRepository.GetById(bookId);

                book.Name = model.Name;
                book.Author = model.Author;
                book.Category = model.Category;
                book.Description = model.Description;
                book.Price = model.Price;
                book.Photo = model.Photo.FileName;

                await _bookRepository.Update(book);
            }

            return RedirectToAction("Books");
        }

        public async Task<IActionResult> DeleteBook(int bookId)
        {
            await _bookRepository.Delete(bookId);
            return RedirectToAction("Books");
        }

    }
}
