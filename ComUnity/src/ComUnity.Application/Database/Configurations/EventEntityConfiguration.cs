using ComUnity.Application.Features.ManagingEvents.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComUnity.Application.Database.Configurations;

internal class EventEntityConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(x => x.EventCategory).WithMany();

        builder.HasOne(x => x.Owner).WithMany().OnDelete(DeleteBehavior.NoAction);
    }
}
