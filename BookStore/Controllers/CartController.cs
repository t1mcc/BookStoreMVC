using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Extensions;
using BookStore.Models;
using BookStore.Models.Cart;
using BookStore.Repositories.Interfaces;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly Cart _cart;
        private readonly IBookRepository _bookRepository;

        public CartController(Cart cart, IBookRepository bookRepository)
        {
            _cart = cart;
            _bookRepository = bookRepository;
        }

        public IActionResult Index()
        {
            return View(_cart);
        }

        public async Task<IActionResult> AddBook(string returnUrl, int bookId, int quantity = 1)
        {
            returnUrl = returnUrl ?? "/";
            var book = await _bookRepository.GetById(bookId);

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
