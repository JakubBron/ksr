using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using KSR_WCF1;

// kontrakt
namespace KSR_WCF1
{
    [ServiceContract]
    public interface IZadanie2
    {
        [OperationContract]
        string Test(string arg);
    }

    // zadanie 7
    [ServiceContract]
    public interface IZadanie7
    {
        [OperationContract]
        [FaultContract(typeof(Wyjatek7))]
        void RzucWyjatek7(string a, int b);
    }

    [DataContract]
    public class Wyjatek7
    {
        [DataMember]
        public string opis;

        [DataMember]
        public string a;

        [DataMember]
        public int b;
    }
}

namespace _2
{
    // klasa serwera
    class server: IZadanie2, IZadanie7
    {
        // usługi serwera
        public string Test(string arg)
        {
            Console.WriteLine("INFO: (server.Test() ) Received arg: " + arg);
            return "Wololo! " + arg;
        }

        // zadanie 7
        public void RzucWyjatek7(string a, int b)
        {
            throw new FaultException<Wyjatek7>(new Wyjatek7
            {
                a = a,
                b = b,
                opis = $"Zadanie 7: {a}, {b}"
            }, new FaultReason("Wyjatek 7 received."));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // powołanie do życia serwera, podanie jego lokalizacji
            var serviceHost = new ServiceHost(typeof(server), new Uri[] {
                new Uri("net.pipe://localhost/ksr-wcf1-zad2"),
                // zadanie 4:
                new Uri("net.tcp://127.0.0.1:55765")    
            });

            // dodanie endpointów - funkcjonalności oferowanych przez usługę i wystawienie ich na świat
            serviceHost.AddServiceEndpoint(typeof(IZadanie2), new NetNamedPipeBinding(), "net.pipe://localhost/ksr-wcf1-zad2");

            // zadanie 4:
            serviceHost.AddServiceEndpoint(typeof(IZadanie2), new NetTcpBinding(), "net.tcp://127.0.0.1:55765");

            // zadanie 7:
            serviceHost.AddServiceEndpoint(typeof(IZadanie7), new NetNamedPipeBinding(), "net.pipe://localhost/ksr-wcf1-zad7");

            // dodanie endpointu od metadanych (task 3)
            var b = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (b == null)
            {
                b = new ServiceMetadataBehavior();
            }
            serviceHost.Description.Behaviors.Add(b);
            serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(),"net.pipe://localhost/metadane");



            // otwieramy serwer - można do niego dzwonić i coś od niego chcieć
            serviceHost.Open();
            Console.WriteLine("INFO: Server started. Press any key to stop server.");
            Console.ReadLine();
            serviceHost.Close();
        }
    }
}
