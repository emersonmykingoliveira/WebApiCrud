using Microsoft.EntityFrameworkCore;

namespace WebApiCrud.Models
{
    public class WebApiContext : DbContext
    {
        public WebApiContext(DbContextOptions<WebApiContext> options)
            : base(options)
        {
        }

        public DbSet<WebApiItem> WebApiItems { get; set; }
    }
}