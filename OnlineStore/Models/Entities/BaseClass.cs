namespace OnlineStore.Models.Entities
{
    namespace OnlineStore.Models.Entities
    {
        public abstract class BaseClass  // ✅ Must be public
        {
            public int Id { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }  // Nullable for soft delete
        }
    }

}
