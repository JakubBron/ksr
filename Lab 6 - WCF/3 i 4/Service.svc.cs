using System.IO;
using System.Xml;

namespace _3
{
    // http://localhost:30703/Service.svc/index.html
    public class Service : IService
    {
        public int Dodaj(string a, string b)
        {
            return int.Parse(a) + int.Parse(b);
        }

        public XmlDocument Index()
        {
            var xml = new XmlDocument();
            xml.Load("C:\\STUDIA PG\\KSR\\Lab 6 - WCF\\3 i 4\\dataToServe\\index.xhtml");
            return xml;
        }

        public Stream Script()
        {
            return File.OpenRead("C:\\STUDIA PG\\KSR\\Lab 6 - WCF\\3 i 4\\dataToServe\\scripts.js");
        }
    }
}
