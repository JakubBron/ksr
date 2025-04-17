using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/**************************************************************/
// IMPORTANT!
// Before running this code, make sure subscribers are running!
/**************************************************************/

namespace publisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("==================================\n\t\t publisher \n==================================\n\n");
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
                await channel.ExchangeDeclareAsync("pub-sub", ExchangeType.Topic);

                for (int i = 0; i < 10; i++)
                {
                    string routingKey = "abc.def";
                    if( i%2 == 1)
                    {
                        routingKey = "abc.xyz"; 
                    }

                    var messageCtx = $"Oto treść mojej wiadmości nr {i}";
                    var body = Encoding.UTF8.GetBytes(messageCtx);

                    await channel.BasicPublishAsync("pub-sub", routingKey, false, body);
                    Console.WriteLine($"Wysłano wiadomość na {routingKey}: {messageCtx}");
                }

            }
        }
    }
}