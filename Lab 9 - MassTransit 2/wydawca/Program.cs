using Wspolne;
using MassTransit;
using MassTransit.Serialization;
using TypyWiadomosci;

var working_status = true;
var stats = new int[5];

var controllerBus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.UseEncryptedSerializer(new AesCryptoStreamProvider(
        new Crypto("19320819320819320819320819320819"), "1932081932081932"));
    cfg.Host("amqps://xkcenflk:9r0uII7CLJfh_zJf76oj1ZvlCI6HHskw@hawk.rmq.cloudamqp.com/xkcenflk");
    cfg.ReceiveEndpoint("control_queue", conf =>
    {
        conf.Handler<Ustaw>(ctx =>
        {
            Console.WriteLine($"[wydawca] Dostal: {ctx.Message}");
            working_status = ctx.Message.Dziala;
            if (!working_status)
            {
                Console.WriteLine("[wydawca] STATS:");
                Console.WriteLine($"\tProby A: {stats[0]}");
                Console.WriteLine($"\tProby B: {stats[1]}");
                Console.WriteLine($"\tOK A: {stats[2]}");
                Console.WriteLine($"\tOK B: {stats[3]}");
                Console.WriteLine($"\tWyslano: {stats[4]}");
            }
            return Task.CompletedTask;
        });
    });
});
var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("amqps://xkcenflk:9r0uII7CLJfh_zJf76oj1ZvlCI6HHskw@hawk.rmq.cloudamqp.com/xkcenflk");
    cfg.ReceiveEndpoint("a_queue", conf =>
    {
        conf.UseMessageRetry(r => r.Immediate(5));
        conf.Handler<OdpA>(ctx =>
        {
            stats[0]++;
            Console.WriteLine($"[wydawca] Odebral (A): {ctx.Message}");
            if (Random.Shared.Next(0, 3) == 0)
            {
                throw new Exception("Oto wyjatek A");
            }
            stats[2]++;
            return Task.CompletedTask;
        });
    });
    cfg.ReceiveEndpoint("b_queue", conf =>
    {
        conf.UseMessageRetry(r => r.Immediate(5));
        conf.Handler<OdpB>(ctx =>
        {
            stats[1]++;
            Console.WriteLine($"[wydawca] Odebral (B): {ctx.Message}");
            if (Random.Shared.Next(0, 3) == 0)
            {
                throw new Exception("Oto wyjatek B");
            }
            stats[3]++;
            return Task.CompletedTask;
        });
    });
});

await controllerBus.StartAsync();
await bus.StartAsync();

do
{
    if (!working_status)
    {
        continue;
    }

    var msg = new Publ(stats[4] + 1);
    Console.WriteLine($"[wydawca] SEND: {msg}");
    await bus.Publish(msg);
    stats[4]++;
} while (true);
