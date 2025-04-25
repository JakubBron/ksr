using MassTransit;
using Messages;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("---------------- odbiorca B ---------------------------------\n\n");
        var handler = new Handler();
        var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("amqps://nqeyilba:u5R6A0h9-iNM7qsWWEN-q2DHtkTG5k79@hawk.rmq.cloudamqp.com/nqeyilba");

            cfg.ReceiveEndpoint(
                "kolejka_dla_odbiorcy__B",
                ec =>
                {
                    ec.Instance(handler);
                }
            );
        });

        await bus.StartAsync();

        Console.ReadKey();
        await bus.StopAsync();
        return;
    }

    internal class Handler : IConsumer<IMessage3>
    {
        private int _counter;

        public Task Consume(ConsumeContext<IMessage3> context)
        {

            Console.WriteLine("Odebrano: ");
            Console.WriteLine(context.Message.messageIM1, "\n");
            Console.WriteLine(context.Message.messageIM2, "\n");

            for (var i = 0; i < context.Headers.Count(); i++)
            {
                Console.WriteLine($"\t {context.Headers.ElementAt(i).Key} : {context.Headers.ElementAt(i).Value}");
            }
            Console.WriteLine($"counter = {++_counter}");
            Console.WriteLine("\n");
            return Task.CompletedTask;
        }
    }
}