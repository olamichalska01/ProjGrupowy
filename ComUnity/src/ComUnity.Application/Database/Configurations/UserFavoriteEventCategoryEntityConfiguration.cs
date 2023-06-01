using ComUnity.Application.Features.UserProfileManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComUnity.Application.Database.Configurations
{
    internal class UserFavoriteEventCategoryEntityConfiguration : IEntityTypeConfiguration<UserFavoriteEventCategory>
    {
        public void Configure(EntityTypeBuilder<UserFavoriteEventCategory> builder)
        {
            // Configure the table name
            builder.ToTable("UserFavoriteEventCategory");

            // Configure the primary key
            builder.HasKey(ufec => new { ufec.UserId, ufec.EventCategoryId });

            // Configure foreign key relationships
            builder.HasOne(ufec => ufec.User)
                .WithMany(u => u.FavoriteCategories)
                .HasForeignKey(ufec => ufec.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ufec => ufec.Category)
                .WithMany()
                .HasForeignKey(ufec => ufec.EventCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
