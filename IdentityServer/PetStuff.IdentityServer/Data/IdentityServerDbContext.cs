using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetStuff.IdentityServer.Models;

namespace PetStuff.IdentityServer.Data
{
    public class IdentityServerDbContext : IdentityDbContext<User, Role, string>
    {
        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options)
            : base(options)
        {
        }
    }
}
