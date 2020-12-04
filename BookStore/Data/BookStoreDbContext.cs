﻿using BookStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Data
{
    public class BookStoreDbContext : IdentityDbContext<User>
    {

        public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();

        }
    }
}
