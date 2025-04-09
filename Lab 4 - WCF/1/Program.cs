using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.ServiceModel;
using System.Runtime.Serialization;
using KSR_WCF1;

// kontrakty - jak gadać z serwerem (libką)?
namespace KSR_WCF1
{
    [ServiceContract]
    public interface IZadanie1
    {
        [OperationContract]
        string Test(string arg);

        // zadanie 5:
        [OperationContract]
        [FaultContract(typeof(Wyjatek))]
        void RzucWyjatek(bool czy_rzucic);

        [OperationContract]
        string OtoMagia(string magia);
    }

    [DataContract]
    public class Wyjatek
    {
        [DataMember]
        public string opis;

        [DataMember]
        public string magia { get; set; }
    }
}

namespace _1
{
    class Program
    {
        static void Main(string[] args)
        {
            // użycie usług serwera
            // !!! najpierw uruchom serwer - uruchom test.exe
            var fact = new ChannelFactory<IZadanie1>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/ksr-wcf1-test"));
            var client = fact.CreateChannel();
            /* Tu można oddziaływać na linii klient (ten prog) <-> serwer */

            
            Console.WriteLine(client.Test("treść testowa"));

            // zadanie 5:
            try
            {
                client.RzucWyjatek(true);
            }
            catch(FaultException<Wyjatek> e)
            {
                Console.WriteLine("WARNING caught exception: " + e.Detail.opis);
                string result = client.OtoMagia(e.Detail.magia);

                // Proszę w przeciągu sekundy wywołać metodę OtoMagia z wartoscią przekazaną w polu magia wyjątku
                Console.WriteLine("Result of client.OtoMagia = " + result);
            }

            (client as IDisposable).Dispose();
            Console.ReadLine();
            fact.Close();
        }
    }
}
