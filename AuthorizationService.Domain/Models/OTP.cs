using System;
namespace AuthorizationService.Domain.Models
{
    public class OTP
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public DateTime Expiry { get; set; }
        public bool IsUsed { get; set; }

        // Внешний ключ и навигационные свойства
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}

