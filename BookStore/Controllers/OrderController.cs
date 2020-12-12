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
        public Cart Cart { get; set; }

        public OrderController(BookStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult PlaceOrder()
        {
            return View(new Order());
        }

        [HttpPost]
        public IActionResult PlaceOrder(Order order) 
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            if (Cart.Items.Count == 0)
            {
                ModelState.AddModelError("", "Ваша корзина пуста!");
            }
            if (ModelState.IsValid)
            {
                order.BookOrders = Cart.Items.Select(book => new BookOrder
                {
                    BookId = book.Book.Id,
                    Quantity = book.Quantity
                }).ToList();

                order.OrderPlaced = DateTime.Now;

                _dbContext.Orders.Add(order);
                _dbContext.SaveChanges();

                Cart.Clear();
                HttpContext.Session.SetJson("cart", Cart);

                return RedirectToAction("Completed");
            }
            return View();
        }

        public IActionResult Completed()
        {
            return View();
        }
    
    }
}
