using Pennyworth.Components;
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
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        public Bodys.Response GetPL(Bodys.Ubications PL)
        {
            Bodys.Response response = Functions.GetPL(PL);
            return response;
        }
        public Bodys.Response GetPBS(Bodys.GetItem PBS)
        {
            Bodys.Response response = Functions.GetPBS(PBS);
            return response;
        }
        public Bodys.Response PostIC(Bodys.InventoryCountings IC)
        {
            //Task<Bodys.Response> response = Functions.PostIC(IC);
            Bodys.Response response = PostIC(IC);
            return response;
        }
        public Bodys.Response PostIP(Bodys.InventoryPostings IP)
        {
            Bodys.Response response = Functions.PostIP(IP);
            return response;
        }
    }
}
