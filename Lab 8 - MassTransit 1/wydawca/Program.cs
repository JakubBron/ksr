using MassTransit;
using Messages;
using wydawca;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("+++++++++++++++++++++++++++++++++ wydawca +++++++++++++++++++++++++++++++\n\n");
        var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("amqps://nqeyilba:u5R6A0h9-iNM7qsWWEN-q2DHtkTG5k79@hawk.rmq.cloudamqp.com/nqeyilba");
        });

        await bus.StartAsync();

        for (var i = 1; i <= 10; i++)
        {
            IMessage1 message1 = new Message1
            {
                messageIM1 = $"Wiadomosc typu Message1 nr {i}"
            };
            var headers1 = new Dictionary<string, string>
            {
                { "id", i.ToString() },
                { "do_kogo", "A, B" },
            };
            await SendMessage(bus, message1, headers1);

            
            IMessage2 message2 = new Message2 
            { 
                messageIM2 = $"Wiadomosc typu Message2 nr {i}"
            };
            var headers2 = new Dictionary<string, string>
            {
                { "id", i.ToString() },
                { "do_kogo", "B, C" },
            };
            await SendMessage(bus, message2, headers2);


            IMessage3 message3 = new Message3
            {
                messageIM1 = $"Wiadomosc typu Message3 nr {i} - treść messageIM1",
                messageIM2 = $"Wiadomosc typu Message3 nr {i} - treść messageIM2"
            };
            var headers3 = new Dictionary<string, string>
            {
                { "id", i.ToString() },
                { "do_kogo", "A, B, C" },
            };
            await SendMessage(bus, message3, headers3);
            
            await Task.Delay(100);
        }

        await bus.StopAsync();
    }

    private static async Task SendMessage<T>(IBusControl bus, T message, Dictionary<string, string> headers)
    {
        Console.WriteLine($"Wysylanie wiadomosci    {message}");
        Console.Write("Headers:");
        for (var i = 0; i < headers.Count; i++)
        {
            Console.Write($"\t {headers.ElementAt(i).Key} : {headers.ElementAt(i).Value}");
        }
        Console.Write("\n");

        await bus.Publish(
            message!,
            context =>
            {
                foreach (var (key, value) in headers)
                {
                    context.Headers.Set(key, value);
                }
            }
        );
    }
}