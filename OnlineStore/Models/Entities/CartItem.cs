namespace OnlineStore.Models.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        // Foreign key for Product
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // Foreign key for Cart
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal Price { get; set; } // Store price at the time of adding
    }

}
