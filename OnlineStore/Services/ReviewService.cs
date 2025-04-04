using OnlineStore.Models.Entities;
using OnlineStore.Models;
using System.Threading.Tasks;

namespace OnlineStore.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddReviewAsync(int productId, int rating, string comment, string userId)
        {
            var review = new Review
            {
                ProductId = productId,
                Rating = rating,
                ReviewText = comment, // Directly use the comment passed in the method
                UserId = userId, // Link review to the user
                Date = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }
    }
}