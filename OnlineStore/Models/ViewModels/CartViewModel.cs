
namespace OnlineStore.Models.ViewModels
{
    public class CartViewModel
    {
        public int CartId { get; set; }
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        // Optionally add more properties like total price or total items if needed
        public decimal TotalPrice => CartItems.Sum(item => item.TotalPrice);
    }
}


