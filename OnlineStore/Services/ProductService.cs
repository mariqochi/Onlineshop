using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Interfaces;
using OnlineStore.Models;
using OnlineStore.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineStore.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(ApplicationDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // ✅ Get all products with filtering
        public async Task<List<Product>> GetAllProductsAsync(
            string search,
            string category,
            string subcategory,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory) // ✅ Fixed SubCategory issue
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category != null && p.Category.Name == category);

            if (!string.IsNullOrEmpty(subcategory))
                query = query.Where(p => p.SubCategory != null && p.SubCategory.Name == subcategory);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query.ToListAsync();
        }

        // ✅ Get product by ID (including reviews)
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // ✅ Add Product (Only Admin)
        public async Task AddProductAsync(Product product)
        {
            if (!IsAdminUser()) throw new UnauthorizedAccessException("Only admins can add products.");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }


        // ✅ Update Product (Only Admin)
        public async Task UpdateProductAsync(Product product)
        {
            if (!IsAdminUser()) throw new UnauthorizedAccessException("Only admins can update products.");

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // ✅ Delete Product (Only Admin)
        public async Task DeleteProductAsync(int id)
        {
            if (!IsAdminUser()) throw new UnauthorizedAccessException("Only admins can delete products.");

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        // ✅ Add Review (Only Logged-in Users)
        public async Task AddReviewAsync(int productId, int rating, string reviewText)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) throw new UnauthorizedAccessException("User must be logged in to write a review.");

            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            var review = new Review
            {
                ProductId = productId,
                Rating = rating,
                ReviewText = reviewText,
                UserId = user.Id,
                Date = DateTime.UtcNow // Use CreatedAt from BaseClass
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        // ✅ Get Current User
        private async Task<AppUser> GetCurrentUserAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            var userId = _userManager.GetUserId(httpContext.User);
            return await _userManager.FindByIdAsync(userId);
        }

        // ✅ Check if the current user is an Admin
        private bool IsAdminUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return false;

            return httpContext.User.IsInRole("Admin");
        }
    }
}
