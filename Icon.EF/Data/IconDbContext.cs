using Icon.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Icon.EF.Data
{
    public class IconDbContext : IdentityDbContext
    {
        public IconDbContext(DbContextOptions<IconDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
    }
}
