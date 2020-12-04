﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class User : IdentityUser
    {
        public ICollection<Order> Orders { get; set; }
    }
}
