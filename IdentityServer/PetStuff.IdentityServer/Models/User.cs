using Microsoft.AspNetCore.Identity;

namespace PetStuff.IdentityServer.Models
{
    public class User : IdentityUser
    {
        // Buraya ek alanlar ekleyebilirsin (zorunlu değil)
        public string? FullName { get; set; }
    }
}
