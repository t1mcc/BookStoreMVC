using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Repositories
{
    public class EFBookRepository : EFRepositoryBase<Book>, IBookRepository
    {
        public EFBookRepository(BookStoreDbContext dbContext) : base(dbContext)
        {

        }

    }
}
