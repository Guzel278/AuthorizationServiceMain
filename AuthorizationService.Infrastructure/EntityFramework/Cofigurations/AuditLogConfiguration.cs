using System;
using AuthorizationService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthorizationService.Infrastructure.EntityFramework.Cofigurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs", "auth_service");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Action)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(a => a.ActionTime)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(a => a.User)
                   .WithMany(u => u.AuditLogs)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // Удаляем логи, если удаляем пользователя
        }
    }
}

