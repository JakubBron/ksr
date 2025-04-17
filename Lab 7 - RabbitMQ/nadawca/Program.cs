using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace nadawca
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("==================================\n\t\tnadawca\n==================================\n\n");
            var connectionFacroty = new ConnectionFactory()
            {
                UserName = "epptlbzm",
                Password = "QTqYDzAuVB-NRWoF680-xrTI-peZUfOJ",
                HostName = "armadillo.rmq.cloudamqp.com",
                VirtualHost = "epptlbzm"
            };

            AsyncEventingBasicConsumer replyConsumer;

            using (IConnection connection = await connectionFacroty.CreateConnectionAsync())
            using (IChannel channel = await connection.CreateChannelAsync())
            {
                string replyQueueName = (await channel.QueueDeclareAsync()).QueueName;
                replyConsumer = new AsyncEventingBasicConsumer(channel);
                replyConsumer.ReceivedAsync += (model, ea) => {
                    var body = ea.Body.ToArray();
                    var text = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Nadawca otrzymał potwierdzenie: {text}");
                    return Task.CompletedTask;
                };
            }


            using (IConnection connection = await connectionFacroty.CreateConnectionAsync())
            using (IChannel channel = await connection.CreateChannelAsync())
            {
                await channel.BasicConsumeAsync(queue: "message_queue", autoAck: true, consumer: replyConsumer);
                for (int i = 0; i < 10; i++)
                {
                    string messageCtx = $"Message no. {i + 1} + some random ctx: {new Random().Next(1, 100)} from 'nadawca'";
                    var body = Encoding.UTF8.GetBytes(messageCtx);

                    // zad. 3 - dodanie nagłówków
                    BasicProperties properties = new BasicProperties();
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("moj_header", $"moj header no. {i} i troche losowosci: {new Random().Next(0, 10)}");
                    properties.Headers.Add("data_utworzenia", DateTime.UtcNow.ToString("s"));



                    await channel.BasicPublishAsync(string.Empty, "message_queue", false, properties, body);
                    Console.WriteLine($"Sent: {messageCtx}");
                    await Task.Delay(1000);
                }
            }
            Console.ReadLine();

            
        }
    }
}