using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderPlaced { get; set; }
        public DateTime? OrderFulfilled { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название страны")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите название города")]
        public string City { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите адрес доставки")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Пожалуйста, введите индекс")]
        public string ZipCode { get; set; }

        public User User { get; set; }
        public ICollection<BookOrder> BookOrders { get; set; }

    }

}
