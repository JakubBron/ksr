using MassTransit;
using MassTransit.Serialization;
using TypyWiadomosci;

var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("amqps://xkcenflk:9r0uII7CLJfh_zJf76oj1ZvlCI6HHskw@hawk.rmq.cloudamqp.com/xkcenflk");
    cfg.ReceiveEndpoint("a_recv_queue", conf =>
    {
        conf.Handler<Publ>(ctx =>
        {
            if (ctx.Message.Numer % 2 != 0)
            {
                return Task.CompletedTask;
            }

            ctx.RespondAsync(new OdpA("A"));
            Console.WriteLine($"[OdbiorcaA] Dostal: {ctx.Message}");
            return Task.CompletedTask;
        });
    });
    cfg.ReceiveEndpoint("a_recv_queue_error", conf =>
    {
        conf.Handler<Fault>(ctx =>
        {
            foreach (var ex in ctx.Message.Exceptions)
            {
                Console.WriteLine($"[OdbiorcaA] BLAD: {ex.Message}");
            }
            return Task.CompletedTask;
        });
    });
});

await bus.StartAsync();

Console.WriteLine($"[OdbiorcaA] Dowolny klawisz by wyjsc.");
Console.ReadLine();

