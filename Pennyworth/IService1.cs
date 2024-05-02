using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Pennyworth
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        [WebInvoke(UriTemplate = "ProductsLocation", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Bodys.Response GetPL(Bodys.Ubications PL);

        [OperationContract]
        [WebInvoke(UriTemplate = "ProductbySKU", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Bodys.Response GetPBS(Bodys.GetItem PBS);

        [OperationContract]
        [WebInvoke(UriTemplate = "InventoryCountings", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Bodys.Response PostIC(Bodys.InventoryCountings IC);

        [OperationContract]
        [WebInvoke(UriTemplate = "InventoryPostings", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Bodys.Response PostIP(Bodys.InventoryPostings IP);
    }


    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.
    
}
