using OnlineStore.Models;
using OnlineStore.Models.Entities;
using System.Threading.Tasks;

public interface IReviewService
{
    Task AddReviewAsync(int productId, int rating, string comment, string userId);
}

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
            ReviewText = comment,
            UserId = userId, // User ID from the controller
            Date = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }
}

