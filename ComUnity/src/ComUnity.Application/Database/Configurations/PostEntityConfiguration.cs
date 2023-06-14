using ComUnity.Application.Features.ManagingEvents.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComUnity.Application.Database.Configurations;

internal class PostEntityConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(x => x.Author).WithMany();
    }
}
