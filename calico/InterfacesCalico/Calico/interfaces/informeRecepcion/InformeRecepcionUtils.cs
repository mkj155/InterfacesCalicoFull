using Calico.interfaces.recepcion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.interfaces.informeRecepcion
{
    class InformeRecepcionUtils
    {
        public static String ObjectToJson(InformeRecepcionJson obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return json;
        }
    }
}
