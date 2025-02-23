using System;
namespace AuthorizationService.Domain.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Навигационные свойства
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}

