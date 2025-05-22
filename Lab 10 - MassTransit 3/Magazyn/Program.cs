using Sklep;
using Magazyn;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<InventoryService>();

        services.AddMassTransit(config =>
        {
            config.AddConsumer<InventoryRequestConsumer>();
            config.AddConsumer<OrderAcceptedConsumer>();
            config.AddConsumer<OrderRejectedConsumer>();

            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("amqps://aoelnfom:mwqcbztFHSwqZJAzkY1BGkdtjzwDg2Su@hawk.rmq.cloudamqp.com/aoelnfom");
                cfg.ReceiveEndpoint("warehouse", e =>
                {
                    e.ConfigureConsumer<InventoryRequestConsumer>(context); ;
                });
                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

Console.WriteLine("[Magazyn] OK. Ctrl+C by wylaczyć usługę.");
await host.RunAsync();
