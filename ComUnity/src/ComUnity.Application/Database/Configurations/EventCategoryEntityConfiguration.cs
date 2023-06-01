using ComUnity.Application.Features.ManagingEvents.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComUnity.Application.Database.Configurations;

internal class EventCategoryEntityConfiguration : IEntityTypeConfiguration<EventCategory>
{
    public void Configure(EntityTypeBuilder<EventCategory> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
