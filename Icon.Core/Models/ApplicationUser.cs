
using Microsoft.AspNetCore.Identity;

namespace Icon.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int UNumber { get; set; }
    }
}
