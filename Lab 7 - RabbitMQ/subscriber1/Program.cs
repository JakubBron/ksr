using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;


namespace subscriber1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("==================================\n\t\t subscriber1 \n==================================\n\n");
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
                string queueName = (await channel.QueueDeclareAsync()).QueueName;

                await channel.ExchangeDeclareAsync("pub-sub", ExchangeType.Topic);
                await channel.QueueBindAsync(queueName, "pub-sub", "abc.*");

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var text = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received: {text} on routingKey: {ea.RoutingKey}");
                };

                await channel.BasicConsumeAsync(queueName, true, consumer);
                Console.ReadLine();
            }
            
        }
    }
}