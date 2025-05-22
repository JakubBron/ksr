using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	public interface IService
	{

        [OperationContract]
        bool Create(string login, string password);

        [OperationContract]
        Guid Login(string login, string password);

        [OperationContract]
        bool Logout(string login);

        [OperationContract]
        bool Put(string name, string content, Guid sessionId);

        [OperationContract]
        string Get(string name, Guid sessionId);
    }
}
