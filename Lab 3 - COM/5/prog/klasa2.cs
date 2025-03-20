using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

/*
            Plan działania
    (!) x86 zamiast All CPU
    0. Naklep kod i wygeneruj klucze: sn.exe -k keyfile.snk
    1. dodaj silną nazwę (klucze): Alt+Enter -> Signing -> Sign the ass... -> dodaj plik .snk
    2. Zbuduj projekt. Skopiuj DLLkę w jakieś sensowne miejsce. Zakładamy, że nazwa = prog.dll
    3. Uruchom Developer Command Prompt... JAKO ADMIN i następnie... 
    4. gacutil /i .\prog.dll
    5. regasm /codebase .\prog.dll
    6. C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe /codebase .\Lab3Klasa2.dll    
    7. Uruchom .\test.exe 2 -> Wololo!!!: Testowanie ProgId KSR20.COM3Klasa.2 ok!
 */


namespace prog
{
    [Guid("F59DA79E-29BB-476C-BFF4-2E9C0ADFDD4D"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IKlasa2
    {
        uint Test(string napis);
    }

    [Guid("F08FB011-E87D-472E-9886-659C2559FB10"), ComVisible(true), ClassInterface(ClassInterfaceType.None), ProgId("KSR20.COM3Klasa.2")]
    public class Klasa2
    {
        public uint Test(string napis)
        {
            Console.WriteLine($"Wololo!!!: {napis}");
            return 0;
        }
    }
}
