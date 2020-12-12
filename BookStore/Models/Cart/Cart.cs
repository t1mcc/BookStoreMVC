using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.Cart
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(Book book, int quantity) 
        {
            var cartItem = Items
                     .Where(item => item.Book.Id == book.Id)
                     .FirstOrDefault();

            if (cartItem == null)
            {
                Items.Add(new CartItem
                {
                    Book = book,
                    Quantity = quantity
                });
            }
            else
            {
                cartItem.Quantity += quantity;
            }
        }
        public void RemoveItem(int bookId) => Items.RemoveAll(item => item.Book.Id == bookId);

        public void Clear() =>  Items.Clear();

        public decimal ComputeTotalPrice() => Items.Sum(item => item.Book.Price * item.Quantity);

    }
}
