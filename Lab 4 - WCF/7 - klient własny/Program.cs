using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using _7___klient_własny.ServiceReference1;


namespace _7___klient_własny
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var client = new ServiceReference1.Zadanie7Client();
            try
            {
                client.RzucWyjatek7("Oto jakaś treść", 400600700);
            }
            catch (FaultException<Wyjatek7> exception)
            {
                Console.WriteLine(exception.Detail.opis);
            }
        }
    }
}