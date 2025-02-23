using System;
using static System.Net.WebRequestMethods;

namespace AuthorizationService.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Внешний ключ и навигационные свойства
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public ICollection<Token> Tokens { get; set; } = new List<Token>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<OTP> OTPs { get; set; } = new List<OTP>();

        // Primary constructor
        public User(string username, string email, string phoneNumber, int roleId)
        {
            Username = username;
            Email = email;
            PhoneNumber = phoneNumber;
            RoleId = roleId;          
        }

        // Для EF Core требуется пустой конструктор
        protected User() { }
    }
}

