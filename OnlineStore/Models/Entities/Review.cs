
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineStore.Models.Entities;
using OnlineStore;
using OnlineStore.Models.Entities.OnlineStore.Models.Entities;

namespace OnlineStore.Models.Entities
{
    public class Review : BaseClass
    {
        public int Id { get; set; }

        // Foreign key to Product
        public int ProductId { get; set; }
        public Product Product { get; set; } // Navigation property

        // Corrected Date property
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow; // Defaults to the current UTC time

        // Rating should be between 1 and 5
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string ReviewText { get; set; }

        // Foreign key to ApplicationUser (User who wrote the review)
        public string UserId { get; set; }
        public AppUser User { get; set; } // Navigation property to ApplicationUser
    }
}





