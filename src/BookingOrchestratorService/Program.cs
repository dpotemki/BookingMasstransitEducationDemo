using BookingOrchestratorService.BookingStateMachines;
using BookingOrchestratorService.Configurations;
using BookingOrchestratorService.Consumers;
using BookingOrchestratorService.Database;
using BookingOrchestratorService.Database.Models;
using FlyghtService.Contracts;
using HotelService.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Contracts;
using TransferService.Contracts;
using UI.Contracts;

namespace BookingOrchestratorService
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            

            var endpointsSection = builder.Configuration.GetSection("EndpointsConfiguration");
            var endpointsConfig = endpointsSection.Get<EndpointsConfiguration>();
            builder.Services.Configure<EndpointsConfiguration>(endpointsSection);

            var rabbitMqSection = builder.Configuration.GetSection("RabbitMqConfiguration");
            var rabbitMqConfig = rabbitMqSection.Get<RabbitMqConfiguration>();

            builder.Services.AddDbContext<StateMachinesDBContext>(bb =>
            {
                bb.UseNpgsql(connectionString);
            });

            var contextOptions = new DbContextOptionsBuilder()
                   .UseNpgsql(connectionString)
                   .Options;
            using (var context = new StateMachinesDBContext(contextOptions))
            {
                context.Database.Migrate();
            }

            builder.Services.AddMassTransit(x =>
            {
                x.AddSagaRepository<BookingState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                        r.ExistingDbContext<StateMachinesDBContext>();
                    });
                x.AddSagaStateMachine<BookingStateMachine, BookingState>()
                //.InMemoryRepository();
                .EntityFrameworkRepository(configure =>
                    {
                        configure.ConcurrencyMode = ConcurrencyMode.Optimistic;
                        configure.ExistingDbContext<StateMachinesDBContext>();
                    });


                //In production you should use a persistent outbox
                //like x.AddEntityFrameworkOutbox...
                x.AddInMemoryInboxOutbox();
                x.AddDelayedMessageScheduler();

                x.AddRequestClient<IBookFlight>(new Uri(endpointsConfig!.FlightServiceQueueName));
                x.AddRequestClient<IBookHotel>(new Uri(endpointsConfig.HotelServiceQueueName));
                x.AddRequestClient<IBookTransfer>(new Uri(endpointsConfig.TransferServiceQueueName));

                x.AddRequestClient<FlyghtBookingConfirmed>();
                x.AddRequestClient<FlyghtBookingRejected>();


                x.AddConsumer<GetAllOrdersInfoConsumer>(
                    cfg =>
                    {
                        cfg.UseDelayedRedelivery(r => r.Intervals(1000, 2000, 4000, 8000, 16000));
                        cfg.UseMessageRetry((r => r.Intervals(1000, 2000, 4000, 8000, 16000)));
                    });

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();
                    cfg.UseJsonSerializer();

                    cfg.ConfigureEndpoints(context);
                    cfg.Host(rabbitMqConfig!.Hostname, rabbitMqConfig.VirtualHost, h =>
                    {
                        h.Username(rabbitMqConfig.Username);
                        h.Password(rabbitMqConfig.Password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
            builder.Services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
                options.StopTimeout = TimeSpan.FromMinutes(1);
            });

            var app = builder.Build();


            app.Run();
        }
    }
}
