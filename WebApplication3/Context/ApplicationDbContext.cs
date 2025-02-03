using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Context
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions opt):base(opt)
        {
            
        }
        public DbSet<Product> Products { get; set; }
    }
}
