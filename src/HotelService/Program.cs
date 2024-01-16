using HotelService.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TransferService.Configuration;
using TransferService.Consumers;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var endpointsSection = builder.Configuration.GetSection("EndpointsConfiguration");
var endpointsConfig = endpointsSection.Get<EndpointsConfiguration>();

var rabbitMqSection = builder.Configuration.GetSection("RabbitMqConfiguration");
var rabbitMqConfig = rabbitMqSection.Get<RabbitMqConfiguration>();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookHotelConsumer>();
    x.AddConsumer<BookHotelCompensateConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseJsonSerializer();
        cfg.ConfigureEndpoints(context);

        cfg.Host(rabbitMqConfig.Hostname, rabbitMqConfig.VirtualHost, h =>
        {
            h.Username(rabbitMqConfig.Username);
            h.Password(rabbitMqConfig.Password);
        });
        cfg.ReceiveEndpoint(endpointsConfig.HotelServiceQueueName, e =>
        {
            e.ConfigureConsumer<BookHotelConsumer>(context);
            e.ConfigureConsumer<BookHotelCompensateConsumer>(context);
        });

        //add some metrics like prometheus
        //cfg.UsePrometheusMetrics(serviceName: "api_service");
    });
});


using IHost host = builder.Build();


await host.RunAsync();