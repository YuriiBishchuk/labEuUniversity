using Lab.DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Lab.DataAccess.Configurations
{
    public class ToDoConfiguration : IEntityTypeConfiguration<ToDo>
    {
        public void Configure(EntityTypeBuilder<ToDo> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(t => t.Description)
                   .HasMaxLength(1000);
            builder.Property(t => t.State)
                   .IsRequired();
            builder.HasOne(t => t.User)
                   .WithMany(u => u.ToDos)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
