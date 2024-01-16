using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingOrchestratorService.Database.Models;

namespace BookingOrchestratorService.Database.Configurations
{
    public class BookingItemStateConfiguration : IEntityTypeConfiguration<BookingItemState>
    {
        public void Configure(EntityTypeBuilder<BookingItemState> builder)
        {
            builder.HasIndex(c => c.Id).IsUnique();
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .IsRequired()
                .ValueGeneratedNever();
        }
    }
}
