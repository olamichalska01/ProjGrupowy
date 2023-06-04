using ComUnity.Application.Features.UserProfileManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComUnity.Application.Database.Configurations
{
    internal class RelationshipEntityTypeConfiguration : IEntityTypeConfiguration<Relationship>
    {
        public void Configure(EntityTypeBuilder<Relationship> builder)
        {
            builder.HasKey(x => new { x.User1Id, x.User2Id });

            builder.HasIndex(x => x.Id);

            builder.HasOne(x => x.User1)
                .WithMany(x => x.Relationships)
                .HasForeignKey(x => x.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User2)
                .WithMany()
                .HasForeignKey(x => x.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}
