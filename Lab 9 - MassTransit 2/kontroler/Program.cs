using Wspolne;
using MassTransit;
using MassTransit.Serialization;
using TypyWiadomosci;

var controllerBus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.UseEncryptedSerializer(new AesCryptoStreamProvider(
       new Crypto("19320819320819320819320819320819"), "1932081932081932"));
    cfg.Host("amqps://xkcenflk:9r0uII7CLJfh_zJf76oj1ZvlCI6HHskw@hawk.rmq.cloudamqp.com/xkcenflk");
});

Console.WriteLine($"[Kontroler] 's' - start | 't' - stop");

while (true)
{
    var key = Console.ReadKey();
    Ustaw? message = null;

    switch (key.Key)
    {
        case ConsoleKey.T:
            message = new Ustaw(false);
            Console.WriteLine("\t Zatrzymanie.");
            break;
        case ConsoleKey.S:
            message = new Ustaw(true);
            Console.WriteLine("\t Start ponowny.");
            break;
    }

    if (message is not null)
    {
        await controllerBus.Publish(message, ctx =>
        {
            ctx.Headers.Set(EncryptedMessageSerializer.EncryptionKeyHeader, Guid.NewGuid().ToString());
        });
    }
}
