using ComUnity.Application.Features.Notifications.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace ComUnity.Application.Database.Configurations;

internal class NotificationEntityConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        var dictionaryConverter = new ValueConverter<Dictionary<string, string>?, string>(
            dictionary => JsonConvert.SerializeObject(dictionary),
            json => JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!
        );

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AdditionalData)
            .HasConversion(dictionaryConverter);
    }
}
