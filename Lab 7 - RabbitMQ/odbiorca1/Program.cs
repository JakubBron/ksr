using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;


namespace odbiorca
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("==================================\n\t\todbiorca 1\n==================================\nAny key to close program.\n");
            var connectionFacroty = new ConnectionFactory()
            {
                UserName = "epptlbzm",
                Password = "QTqYDzAuVB-NRWoF680-xrTI-peZUfOJ",
                HostName = "armadillo.rmq.cloudamqp.com",
                VirtualHost = "epptlbzm"
            };

            using (IConnection connection = await connectionFacroty.CreateConnectionAsync())
            using (IChannel channel = await connection.CreateChannelAsync())
            {
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var text = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received: {text}");

                    // zad.3 - odczytanie nagłówków
                    var headers = ea.BasicProperties.Headers;
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            Console.WriteLine($"Header: {header.Key} - {Encoding.UTF8.GetString((byte[])header.Value)}");
                        }
                    }
                    await Task.Delay(2000);
                    // zad. 5
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    Console.WriteLine("\n");
                };
                await channel.BasicConsumeAsync("message_queue", false, consumer);

                Console.ReadLine();
            }
        }
    }
}