namespace OnlineStore.Models.Entities;
public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty; // ✅ FIXED: Initialized to avoid null error

    // Navigation properties
    public List<SubCategory> SubCategories { get; set; } = new(); // ✅ Initialize to avoid null collection
    public List<Product> Products { get; set; } = new();          // ✅ Initialize to avoid null collection
}

