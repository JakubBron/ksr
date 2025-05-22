using Sklep;
using MassTransit;
using Microsoft.Extensions.Hosting;

var repo = new InMemorySagaRepository<OrderState>();
var machine = new OrderStateMachine();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("amqps://aoelnfom:mwqcbztFHSwqZJAzkY1BGkdtjzwDg2Su@hawk.rmq.cloudamqp.com/aoelnfom");
                cfg.ReceiveEndpoint("OrderState", e =>
                {
                    e.StateMachineSaga(machine, repo);
                });
                cfg.UseInMemoryScheduler();
            });
        });
    })
    .Build();

Console.WriteLine("Sklep OK. Ctrl+C aby wyjść.");
await host.RunAsync();