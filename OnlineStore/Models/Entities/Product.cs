using OnlineStore.Models.Entities;
using System.Collections.Generic;

namespace OnlineStore.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }

        // Foreign Key to Category
        public int CategoryId { get; set; }  // Nullable to allow SetNull behavior
        public Category Category { get; set; }  // Navigation property to Category

        // Foreign Key to Subcategory
        public int? SubCategoryId { get; set; }  // Nullable to allow Restrict or SetNull behavior
        public SubCategory SubCategory { get; set; }  // Navigation property to Subcategory
      
        public string Name { get; set; } // Product name (Brand + Model)
        public string BriefDescription { get; set; }
        public decimal Price { get; set; }
        public double Rating { get; set; }  // Can be calculated from reviews
        public string ImageUrl { get; set; }

        // Navigation property to link reviews to the product
        public List<Review> Reviews { get; set; }
    }
}
