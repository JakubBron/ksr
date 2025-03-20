using System;
using System.Reflection;

class Program
{
    public static void Main(string[] args)
    {
        Type type = Type.GetTypeFromProgID("KSR20.COM3Klasa.1");
        object arg = "21 jb";


        if (type != null)
        {
            Console.WriteLine("COM istnieje...");
            try
            {
                object act = Activator.CreateInstance(type);

                if (arg is string)
                {
                    type.InvokeMember("Test", System.Reflection.BindingFlags.InvokeMethod, null, act, new object[] { arg } );
                }
                else
                {
                    Console.WriteLine("param nie jest stringiem");
                }
            }
            catch
            {
                Console.WriteLine("cos poszlo nie tak");
            }
        }
        else
        {
            Console.WriteLine("nie pobrano typu");
        }
    }
}