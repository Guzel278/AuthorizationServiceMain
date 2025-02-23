using System;
namespace AuthorizationService.Domain.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = null!;
        public DateTime ActionTime { get; set; }

        // Внешний ключ и навигационные свойства
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}

