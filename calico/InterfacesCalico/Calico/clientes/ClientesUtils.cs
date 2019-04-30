using Calico.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.clientes
{
    class ClientesUtils
    {
        public String buildUrl(String urlParam, String key, String fecha)
        {
            String url = String.Empty;
            Dictionary<String, String> dictionary = new Dictionary<string, string>();
            if (Constants.MLNM.Equals(key))
            {
                dictionary.Add(Constants.PARAM_FECHA, fecha);
                url = Utils.buildUrl(urlParam, dictionary);
            }
            else if (Constants.TAX.Equals(key))
            {
                dictionary.Add(Constants.PARAM_FECHA, fecha);
                url = Utils.buildUrl(urlParam, dictionary);
            }

            return url;
        }
    }
}
