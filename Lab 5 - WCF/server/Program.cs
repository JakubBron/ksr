using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

using KSR_WCF2;         // and dont forget to add .dll files as reference

namespace server
{
    class Program
    {
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
        private class Server : IZadanie3, IZadanie4
        {
            private int _counter = 0;

            public void TestujZwrotny()
            {
                var channel = OperationContext.Current.GetCallbackChannel<IZadanie3Zwrotny>();
                for (var x = 0; x <= 30; x++)
                {
                    channel.WolanieZwrotne(x, x * x * x - x * x);
                }
            }

            public void Ustaw(int v)
            {
                _counter = v;
            }

            public int Dodaj(int v)
            {
                _counter += v;
                return _counter;
            }
        }

        public static void Main(string[] args)
        {
            var host = new ServiceHost(typeof(Server));
            var b = host.Description.Behaviors.Find<ServiceMetadataBehavior>() ?? new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(b);
            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), "net.pipe://localhost/metadane");
            host.AddServiceEndpoint(typeof(IZadanie3), new NetNamedPipeBinding(), "net.pipe://localhost/ksr-wcf2-zad3");
            host.AddServiceEndpoint(typeof(IZadanie4), new NetNamedPipeBinding(), "net.pipe://localhost/ksr-wcf2-zad4");

            host.Open();
            Console.WriteLine("Server started. Press [Enter] to exit.");
            Console.ReadLine();
            host.Close();
        }
    }
}
