using MassTransit;
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
    public class BookingStateMap : SagaClassMap<BookingState>
    {
        protected override void Configure(EntityTypeBuilder<BookingState> entity, ModelBuilder model)
        {
            entity.HasIndex(c => c.CorrelationId).IsUnique();
            entity.HasKey(c => c.CorrelationId);

            entity.HasMany(o => o.BookingItemStates)
                .WithOne(c => c.BookingState)
                .HasForeignKey(c => c.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(c => c.BookingItemStates).AutoInclude();
        }
    }
}
