using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Extensions;
using BookStore.Models;
using BookStore.Models.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        BookStoreDbContext _dbContext;
        Cart _cart;

        public OrderController(BookStoreDbContext dbContext, Cart cart)
        {
            _dbContext = dbContext;
            _cart = cart;
        }

        [HttpGet]
        public IActionResult PlaceOrder()
        {
            return View(new Order());
        }

        [HttpPost]
        public IActionResult PlaceOrder(Order order) 
        {
            if (_cart.Items.Count == 0)
            {
                ModelState.AddModelError("", "Ваша корзина пуста!");
            }
            if (ModelState.IsValid)
            {
                order.BookOrders = _cart.Items.Select(book => new BookOrder
                {
                    BookId = book.Book.Id,
                    Quantity = book.Quantity
                }).ToList();

                order.OrderPlaced = DateTime.Now;

                _dbContext.Orders.Add(order);
                _dbContext.SaveChanges();

                _cart.Clear();

                return RedirectToAction("Completed");
            }
            return View();
        }

        public IActionResult SetOrderFulfield(int orderId, string returnUrl) 
        {
            var order = _dbContext.Orders.FirstOrDefault(x => x.Id == orderId);
            order.OrderFulfilled = DateTime.Now;

            _dbContext.SaveChanges();

            return Redirect(returnUrl);
        }

        public IActionResult Completed()
        {
            return View();
        }
    
    }
}
