using ComUnity.Application.Features.Authentication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComUnity.Application.Database.Configurations;

internal class AuthenticationUserEntityConfiguration : IEntityTypeConfiguration<AuthenticationUser>
{
    public void Configure(EntityTypeBuilder<AuthenticationUser> builder)
    {
        builder.Ignore(e => e.DomainEvents);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Email);

        builder.HasIndex(x => x.SecurityCode);

        builder.Property(x => x.Email)
                .HasMaxLength(320)
                .IsRequired();

        builder.Property(x => x.HashedPassword)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.SecurityCode)
            .HasMaxLength(64)
            .IsRequired(false);
    }
}
