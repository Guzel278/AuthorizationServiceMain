using System;
using AuthorizationService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthorizationService.Infrastructure.EntityFramework.Cofigurations
{
    public class OTPConfiguration : IEntityTypeConfiguration<OTP>
    {
        public void Configure(EntityTypeBuilder<OTP> builder)
        {
            builder.ToTable("OTPs", "auth_service");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Code)
                   .IsRequired()
                   .HasMaxLength(6); 

            builder.Property(o => o.Expiry)
                   .IsRequired();

            builder.Property(o => o.IsUsed)
                   .HasDefaultValue(false);

            builder.HasOne(o => o.User)
                   .WithMany(u => u.OTPs)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // Удаляем OTP, если удаляем пользователя
        }
    }
}

