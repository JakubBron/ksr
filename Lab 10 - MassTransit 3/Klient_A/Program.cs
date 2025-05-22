using MessageTypesDefinition;
using Klient_A;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumer<ConfirmationRequestConsumer>();
            config.AddConsumer<OrderAcceptedConsumer>();
            config.AddConsumer<OrderRejectedConsumer>();

            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("amqps://aoelnfom:mwqcbztFHSwqZJAzkY1BGkdtjzwDg2Su@hawk.rmq.cloudamqp.com/aoelnfom");

                cfg.ReceiveEndpoint($"client-A", e =>
                {
                    e.ConfigureConsumer<ConfirmationRequestConsumer>(context);
                    e.ConfigureConsumer<OrderAcceptedConsumer>(context);
                    e.ConfigureConsumer<OrderRejectedConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

var busControl = host.Services.GetRequiredService<IBusControl>();
await busControl.StartAsync();

Console.WriteLine($"[A] 'P' by zlozyc zam.  'W' - wyjdz.");

while (true)
{
    var key = Console.ReadKey(true).Key;

    if (key == ConsoleKey.W)
    {
        break;
    }

    if (key == ConsoleKey.P)
    {
        Console.Write("Podaj ilosc: ");
        if (int.TryParse(Console.ReadLine(), out var quantity) && quantity > 0)
        {
            var sendEndpoint = await busControl.GetPublishSendEndpoint<StartOrder>();
            await sendEndpoint.Send(new StartOrder
            {
                Quantity = quantity,
                ClientId = "A"
            });
            Console.WriteLine($"Zam. na {quantity} szt: OK.");
        }
        else
        {
            Console.WriteLine("Niepoprawna wartość!");
        }
    }
}

await busControl.StopAsync();
