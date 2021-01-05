using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Extensions;
using BookStore.Models;
using BookStore.Models.Cart;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        BookStoreDbContext _dbContext;
        Cart _cart;

        public CartController(BookStoreDbContext dbContext, Cart cart)
        {
            _dbContext = dbContext;
            _cart = cart;
        }

        public IActionResult Index()
        {
            return View(_cart);
        }

        public IActionResult AddBook(string returnUrl, int bookId, int quantity = 1)
        {
            returnUrl = returnUrl ?? "/";
            Book book = _dbContext.Books
                        .FirstOrDefault(book => book.Id == bookId);

            _cart.AddItem(book, quantity);

            return Redirect(returnUrl);
        }

        public IActionResult RemoveItem(int bookId) {
            _cart.RemoveItem(bookId);

            return RedirectToAction("Index");
        }

        public IActionResult Clear() {
            _cart.Clear();

            return RedirectToAction("Index");
        }

    }
}
