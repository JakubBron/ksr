using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Discovery;
using System.ServiceModel;

namespace _2
{
    [ServiceContract]
    interface IUsluga
    {
        [OperationContract]
        string ScalNapisy(string a, string b);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client: searching for endpoints...");
            DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint("soap.udp://localhost:30703"));
            System.Collections.ObjectModel.Collection<EndpointDiscoveryMetadata> lst = discoveryClient.Find(new FindCriteria(typeof(IUsluga))).Endpoints;
            discoveryClient.Close(); 
            if (lst.Count > 0)
            {
                Console.WriteLine("Client: FOUND!");
                var addr = lst[0].Address; //łączymy się z pierwszym znalezionym
                var proxy = ChannelFactory<IUsluga>.CreateChannel(new NetNamedPipeBinding(), addr);
                var result = proxy.ScalNapisy("KSR", "Laboratorium");
                ((IDisposable)proxy).Dispose();
                Console.WriteLine($"Client used ScalNapisy on remote server. Result is: {result}");
            }


        }
    }
}
