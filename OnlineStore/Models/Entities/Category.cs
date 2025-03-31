namespace OnlineStore.Models.Entities;
     public class Category
     {
        public int Id { get; set; }
        public string Name { get; set; }
           
        // Navigation property to link Subcategories and Products
        public List<SubCategory> SubCategories { get; set; }
        public List<Product> Products { get; set; }
     }

