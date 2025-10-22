using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace EtiWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Orders> Orders { get; set; } = default!;
        public DbSet<Request> Request { get; set; } = default!;
        public DbSet<RequestFile> RequestFile { get; set; } = default!;
        public DbSet<SalesOrders> SalesOrders { get; set; } = default!;

        public DbSet<RequestFile> vw_RequestDetails { get; set; } = default!;
    }
}
