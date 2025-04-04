namespace OnlineStore.Models.ViewModels
{
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public string? ProductName { get; set; } 
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Quantity * Price; // Calculate total price for the cart item
    }
}
