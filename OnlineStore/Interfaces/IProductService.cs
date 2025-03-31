using OnlineStore.Models.Entities;

namespace OnlineStore.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync(string search, string category, string name, decimal? minPrice, decimal? maxPrice);
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task AddReviewAsync(int productId, int rating, string reviewText);

        
    }

}



