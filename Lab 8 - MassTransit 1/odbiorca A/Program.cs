using MassTransit;
using Messages;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("---------------- odbiorca A ---------------------------------\n\n");
        var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("amqps://nqeyilba:u5R6A0h9-iNM7qsWWEN-q2DHtkTG5k79@hawk.rmq.cloudamqp.com/nqeyilba");

            cfg.ReceiveEndpoint(
                "kolejka_dla_odbiorcy__A",
                ec =>
                {
                    ec.Handler<IMessage1>(Handle);
                }
            );
        });

        await bus.StartAsync();

        Console.ReadKey();
        await bus.StopAsync();
        return;
    }

    private static Task Handle(ConsumeContext<IMessage1> context)
    {
        Console.WriteLine("Odebrano: ");
        Console.Write(context.Message.messageIM1, "\n");

        for(var i = 0; i < context.Headers.Count(); i++)
        {
            Console.Write($"\t {context.Headers.ElementAt(i).Key} : {context.Headers.ElementAt(i).Value}");
        }
        Console.Write("\n");
        return Task.CompletedTask;
    }
}