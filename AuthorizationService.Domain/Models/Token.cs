namespace AuthorizationService.Domain.Models
{
    public class Token
    {
        public int Id { get; set; }
        public string TokenValue { get; set; } = null!;
        public DateTime Expiry { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; }

        // Внешний ключ и навигационные свойства
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}

