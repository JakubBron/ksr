using _1.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace _1
{
    class Program
    {

        public static async Task Main(string[] args)
        {
            await Zadanie1();
            Zadanie2();
        }
        
        private static async Task Zadanie1()
        {
            var client = new Zadanie1Client();
            var long_task = client.DlugieObliczeniaAsync();
            for (var x = 0; x <= 20; x++)
            {
                client.Szybciej(x, 3 * x * x - 2 * x);
            }
            
            await long_task;
        }

        private static void Zadanie2()
        {
            var client = new Zadanie2Client(new InstanceContext(new Zadanie2Handler()));
            client.PodajZadania();
            Console.WriteLine("Click [ENTER] to stop listening");
            Console.Read();
        }

        private class Zadanie2Handler : IZadanie2Callback
        {
            public void Zadanie(string zadanie1, int pkt, bool zaliczone)
            {
                Console.WriteLine($"Podzadanie {zadanie1}, {pkt}, {zaliczone}");
            }
        }

        private class ServiceReference1 : IZadanie2Callback
        {
            public void Zadanie(string zadanie1, int pkt, bool zaliczone)
            {
                Console.WriteLine($"Podzadanie {zadanie1}, {pkt}, {zaliczone}");
            }
        }

    }
}
