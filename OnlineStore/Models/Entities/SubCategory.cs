namespace OnlineStore.Models.Entities
{
    public class SubCategory 
    {
        public int Id { get; set; }
        public string Name { get; set; } // Name of the subcategory (e.g., Mobile Phones)

        // Foreign Key to Category
        public int CategoryId { get; set; }
        public Category Category { get; set; }  // Navigation property to Category

        // Navigation property to link products to the subcategory
        public List<Product> Products { get; set; }
    }

}
