using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml;

namespace _3
{
	[ServiceContract]
	public interface IService
	{

		[OperationContract]
        [WebGet(UriTemplate = "scripts.js")]
        Stream Script();

        [OperationContract]
		[WebGet(UriTemplate = "index.html")]
        [XmlSerializerFormat]
        XmlDocument Index();

        [OperationContract]
        [WebInvoke(UriTemplate = "Dodaj/{a}/{b}")]
        int Dodaj(string a, string b);


    }

}
