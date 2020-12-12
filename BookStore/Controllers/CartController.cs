using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Extensions;
using BookStore.Models;
using BookStore.Models.Cart;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        BookStoreDbContext _dbContext;
        public Cart Cart { get; set; }

        public CartController(BookStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            return View(Cart);
        }

        public IActionResult AddBook(string returnUrl, int bookId, int quantity = 1)
        {
            returnUrl = returnUrl ?? "/";

            Book book = _dbContext.Books
                        .FirstOrDefault(book => book.Id == bookId);

            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            Cart.AddItem(book, quantity);
            HttpContext.Session.SetJson("cart", Cart);

            return Redirect(returnUrl);
        }

        public IActionResult RemoveItem(int bookId) {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            Cart.RemoveItem(bookId);
            HttpContext.Session.SetJson("cart", Cart);

            return RedirectToAction("Index");
        }

        public IActionResult Clear() {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            Cart.Clear();
            HttpContext.Session.SetJson("cart", Cart);

            return RedirectToAction("Index");
        }

    }
}
