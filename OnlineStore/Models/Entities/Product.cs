using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }

        // Foreign Key to Category
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        // Foreign Key to Subcategory
        public int? SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public SubCategory? SubCategory { get; set; }

        public string Name { get; set; }
        public string BriefDescription { get; set; }
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public string ImageUrl { get; set; }

        public List<Review> Reviews { get; set; }
    }
}
