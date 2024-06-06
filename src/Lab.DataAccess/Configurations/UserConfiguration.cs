using Lab.DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.FirstName).HasMaxLength(100);
            builder.Property(u => u.LastName).HasMaxLength(100);
            builder.Property(u => u.Username).HasMaxLength(100);
            builder.Property(u => u.Password).HasMaxLength(100);
            builder.Property(u => u.Token).HasMaxLength(200);
            builder.Property(u => u.Role).HasMaxLength(50);
            builder.Property(u => u.Email).HasMaxLength(100);
            builder.Property(u => u.RefreshToken).HasMaxLength(200);
            builder.Property(u => u.RefreshTokenExpiryTime).IsRequired();
            builder.HasMany(u => u.ToDos)
                   .WithOne(t => t.User)
                   .HasForeignKey(t => t.UserId);
        }
    }
}
