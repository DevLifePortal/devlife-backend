using DevLife.Infrastructure.Models.Entities;
using DevLife.Infrastructure.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevLife.Infrastructure.Database.Postgres.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> userBuilder)
    {
        userBuilder.ToTable("Users");

        userBuilder.HasKey(x => x.Id);

        userBuilder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(30);

        userBuilder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(50);

        userBuilder.Property(x => x.Birthday)
            .IsRequired();

        userBuilder
            .Property(x => x.ZodiacSign)
            .HasConversion(
                z => z.Name,
                name => new ZodiacSign(name)
            )
            .IsRequired()
            .HasMaxLength(20);

        userBuilder.Property(x => x.TechStack)
            .HasConversion<string>() 
            .IsRequired();

        userBuilder.Property(x => x.ExperienceLevel)
            .HasConversion<string>()
            .IsRequired();

        userBuilder.Property(x => x.Balance)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        userBuilder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()"); 
    }
}