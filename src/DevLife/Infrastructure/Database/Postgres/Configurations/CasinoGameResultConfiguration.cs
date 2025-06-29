using DevLife.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevLife.Infrastructure.Database.Postgres.Configurations;

public class CasinoGameResultConfiguration : IEntityTypeConfiguration<CasinoGameResult>
{
    public void Configure(EntityTypeBuilder<CasinoGameResult> casinoGameResultbuilder)
    {
        casinoGameResultbuilder.ToTable("Casino_Game_Results");

        casinoGameResultbuilder.HasKey(x => x.Id);

        casinoGameResultbuilder.Property(x => x.UserId)
            .IsRequired();

        casinoGameResultbuilder.Property(x => x.UserAnswer)
            .HasConversion<string>() 
            .IsRequired();

        casinoGameResultbuilder.Property(x => x.CorrectAnswer)
            .HasConversion<string>()
            .IsRequired();

        casinoGameResultbuilder.Property(x => x.IsCorrect)
            .IsRequired();

        casinoGameResultbuilder.Property(x => x.Bet)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        casinoGameResultbuilder.Property(x => x.Points)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        casinoGameResultbuilder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()");
        
        casinoGameResultbuilder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
    }
}