using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RedisSaple.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public UserType UserType { get; set; }
    }

    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(25);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(25);
            builder.Property(x => x.Password).IsRequired().HasMaxLength(25);
            builder.Property(x => x.UserType).IsRequired();
        }
    }

    public enum UserType {
        User,
        Admin,
        Auditor
    }
}
