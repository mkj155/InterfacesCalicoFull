using Calico.persistencia;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.interfaces.informePedido
{
    class InformePedidoUtils
    {

        public static String LAST_ERROR = String.Empty;

        internal static List<InformePedidoJson> MappingInforme(tblInformePedido informe, String OrderCompany, String OrderType)
        {
            return null;
        }

        public static String JsonToString(InformePedidoJson obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return json;
        }

        public static InformePedidoJson GetObjectJsonFromDTO(InformePedidoDTO detalle)
        {
            List<InformePedidoDTO> list = new List<InformePedidoDTO>();
            list.Add(detalle);
            return new InformePedidoJson(list);
        }

        public static bool ExistChildrenInJson(String jsonString, String father, String children)
        {
            return false;
        }

        public static Boolean SendRequestPost(string url, String user, String pass, String json)
        {
            return false;
        }

        public static void handleErrorRest(String myJsonString, out string error)
        {
            JObject json = JObject.Parse(myJsonString);

            Console.WriteLine("Servicio Rest KO");
            Console.WriteLine();
            Console.WriteLine("Detalle: ");
            Console.WriteLine(json["message"]);
            error = json["message"].ToString();
        }

    }
}
