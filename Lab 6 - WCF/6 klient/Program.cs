using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace _6_klient
{
    [ServiceContract]
    interface IUsluga
    {
        [OperationContract]
        int Dodaj(int a, int b);
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new ChannelFactory<IUsluga>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/6"));
            var channel = factory.CreateChannel();
            while (true)
            {
                Console.WriteLine($"Result of 10+15 is {channel.Dodaj(10, 15)}");
                await Task.Delay(1000);
            }
        }
    }
}