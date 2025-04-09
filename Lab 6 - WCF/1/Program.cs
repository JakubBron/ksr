using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Discovery;


namespace _1
{
    [ServiceContract]
    interface IUsluga
    {
        [OperationContract]
        string ScalNapisy(string a, string b);
    }

    class Server: IUsluga
    {
        public string ScalNapisy(string a, string b)
        {
            return $"{a}{b}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var sh = new ServiceHost(typeof(Server));
            sh.Description.Behaviors.Add(new ServiceDiscoveryBehavior());

            sh.AddServiceEndpoint(new UdpDiscoveryEndpoint("soap.udp://localhost:30703"));
            sh.AddServiceEndpoint(typeof(IUsluga), new NetNamedPipeBinding(), "net.pipe://localhost/usluga");


            sh.Open();
            Console.WriteLine("Server is working");
            Console.ReadLine();
            sh.Close();
        }
    }
}
