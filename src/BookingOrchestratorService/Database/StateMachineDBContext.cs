using BookingOrchestratorService.Database.Configurations;
using BookingOrchestratorService.Database.Models;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingOrchestratorService.Database
{
    public class StateMachinesDBContext : SagaDbContext
    {

        public DbSet<BookingState> BookingStates { get; set; }
        public DbSet<BookingItemState> ServiceStates { get; set; }
        public StateMachinesDBContext(DbContextOptions options) : base(options)
        {


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookingItemStateConfiguration());

            base.OnModelCreating(modelBuilder);

        }

        protected override IEnumerable<ISagaClassMap> Configurations { get { yield return new BookingStateMap(); } }
    }
}
