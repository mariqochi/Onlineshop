namespace OnlineStore.Models.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;


    public class Cart
    {
        public int Id { get; set; }

        // Foreign key for User (Required)
        public string UserId { get; set; }
        public AppUser User { get; set; }  // Not nullable, because a cart must belong to a user

        // Navigation property for cart items
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

    }


}
