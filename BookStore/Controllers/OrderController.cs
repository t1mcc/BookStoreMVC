using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Extensions;
using BookStore.Models;
using BookStore.Models.Cart;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly Cart _cart;
        private readonly IOrderRepository _orderRepository;

        public OrderController(Cart cart, IOrderRepository orderRepository)
        {
            _cart = cart;
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public IActionResult PlaceOrder()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Order order) 
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

                await _orderRepository.Add(order);

                _cart.Clear();

                return RedirectToAction("Completed");
            }
            return View();
        }

        public async Task<IActionResult> SetOrderFulfield(int orderId, string returnUrl) 
        {
            var order = await _orderRepository.GetById(orderId);
            order.OrderFulfilled = DateTime.Now;

            await _orderRepository.Update(order);

            return Redirect(returnUrl);
        }

        public IActionResult Completed()
        {
            return View();
        }
    
    }
}
