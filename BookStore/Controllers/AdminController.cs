using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
            var orders = GetOrders(x => x.OrderFulfilled == null);

            return View(orders);
        }

        public IActionResult CompletedOrders() 
        {
            var orders = GetOrders(x => x.OrderFulfilled != null);

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

        public IActionResult Books()
        {
            var books = _dbContext.Books.ToList();
            return View(books);
        }

        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBook(BookViewModel model)
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

                _dbContext.Books.Add(book);
                _dbContext.SaveChanges();
            }

            return View();
        }

        public IActionResult EditBook(int bookId) 
        {
            ViewBag.bookId = bookId;
            var book = _dbContext.Books.FirstOrDefault(book => book.Id == bookId);
            
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
        public IActionResult EditBook(BookViewModel model, int bookId)
        {
            if (ModelState.IsValid)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", model.Photo.FileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }

                var book = _dbContext.Books.FirstOrDefault(book => book.Id == bookId);

                book.Name = model.Name;
                book.Author = model.Author;
                book.Category = model.Category;
                book.Description = model.Description;
                book.Price = model.Price;
                book.Photo = model.Photo.FileName;

                _dbContext.Books.Update(book);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Books");
        }

        public IActionResult DeleteBook(int bookId)
        {
            var book = _dbContext.Books.FirstOrDefault(book => book.Id == bookId);

            _dbContext.Books.Remove(book);
            _dbContext.SaveChanges();

            return RedirectToAction("Books");
        }

        private IEnumerable<Order> GetOrders(Expression<Func<Order, bool>> @where)
        {
            return _dbContext.Orders.Where(where)
                              .Include("User")
                              .Include("BookOrders")
                              .Include("BookOrders.Book")
                              .ToList();
        }
    }
}
