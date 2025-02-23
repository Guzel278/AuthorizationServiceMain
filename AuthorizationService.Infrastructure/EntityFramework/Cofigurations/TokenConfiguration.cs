using System;
using AuthorizationService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthorizationService.Infrastructure.EntityFramework.Cofigurations
{
    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.ToTable("Tokens", "auth_service");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.TokenValue)
                   .IsRequired();

            builder.Property(t => t.Expiry)
                   .IsRequired();

            builder.Property(t => t.IsRevoked)
                   .HasDefaultValue(false);

            builder.Property(t => t.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(t => t.User)
                   .WithMany(u => u.Tokens)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // Удаляем токены, если удаляем пользователя
        }
    }
}

