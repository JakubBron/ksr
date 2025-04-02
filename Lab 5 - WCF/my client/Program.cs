using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using my_client.ServiceReference1;


namespace my_client
{
    class Program
    {
        private class Handler : IZadanie6Callback
        {
            public void Wynik(int wyn)
            {
                Console.WriteLine($"Zadanie 6 (dodawanie): {wyn}");
            }
        }

        static void Main(string[] args)
        {
            var client5 = new Zadanie5Client();
            Console.WriteLine($"Zadanie 5 (scalanie): {client5.ScalNapisy("lubie ", "uczelnie")}");

            var client6 = new Zadanie6Client(new InstanceContext(new Handler()));
            client6.Dodaj(200, 3400);

            Console.WriteLine("Any key to stop...");
            Console.Read();
        }
    }
}