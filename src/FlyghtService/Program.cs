using FlyghtService.Consumers;
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
    x.AddConsumer<BookFlyghtConsumer>();//.Endpoint(z=> z.Name = endpointsConfig!.TransferServiceQueueName);
    x.AddConsumer<BookFlyghtCompensateConsumer>();//.Endpoint(z => z.Name = endpointsConfig!.TransferServiceQueueName);
    x.AddInMemoryInboxOutbox();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseJsonSerializer();
        cfg.ConfigureEndpoints(context);

        cfg.Host(rabbitMqConfig!.Hostname, rabbitMqConfig.VirtualHost, h =>
        {
            h.Username(rabbitMqConfig.Username);
            h.Password(rabbitMqConfig.Password);
        });
        cfg.ReceiveEndpoint(endpointsConfig!.TransferServiceQueueName, e =>
        {
            e.UseMessageRetry(r => r.Intervals(500, 1000, 2000));
            e.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15), TimeSpan.FromMinutes(1)));

            e.ConfigureConsumer<BookFlyghtConsumer>(context);
            e.ConfigureConsumer<BookFlyghtCompensateConsumer>(context);
        });

        //add some metrics like prometheus
        //cfg.UsePrometheusMetrics(serviceName: "api_service");
    });
});


using IHost host = builder.Build();


await host.RunAsync();