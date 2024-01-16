using MassTransit;
using UI.Contracts;
using UiApiService.Models.Configurations;

namespace UiApiService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var routingSection = builder.Configuration.GetSection("RoutingConfiguration");
            var routingConfig = routingSection.Get<RoutingConfiguration>();

            var rabbitMqSection = builder.Configuration.GetSection("RabbitMqConfiguration");
            var rabbitMqConfig = rabbitMqSection.Get<RabbitMqConfiguration>();




            builder.Services.AddMassTransit(x =>
            {
                x.AddDelayedMessageScheduler();

                x.AddRequestClient<GetAllOrdersInfoState>();


                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseJsonSerializer();
                    cfg.ConfigureEndpoints(context);

                    cfg.Host(rabbitMqConfig.Hostname, rabbitMqConfig.VirtualHost, h =>
                    {
                        h.Username(rabbitMqConfig.Username);
                        h.Password(rabbitMqConfig.Password);
                    });

                    //add some metrics like prometheus
                    //cfg.UsePrometheusMetrics(serviceName: "api_service");
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
