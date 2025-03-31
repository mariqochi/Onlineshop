using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace OnlineStore.Models.Entities
{
    public class AppUser : IdentityUser
    {
        // Navigation property: A user can have multiple reviews
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
