using Calico.common;
using Calico.persistencia;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.interfaces.recepcion
{
    class RecepcionUtils
    {
        public String BuildUrl(String urlParam, String fecha)
        {
            Dictionary<String, String> dictionary = new Dictionary<string, string>();
            dictionary.Add(Constants.PARAM_FECHA, fecha);
            return Utils.BuildUrl(urlParam, dictionary);
        }

        private String GetHeaderJson()
        {

            return Constants.JSON_PREFIX + Constants.JSON_SUBFIX_RECEPTION;
        }

        public void MappingRecepcion(String myJsonString,Dictionary<String, tblRecepcion> diccionary)
        {
            //var json = JObject.Parse(myJsonString);
            //var root = json[GetHeaderJson()];
            //var data = root[Constants.JSON_TAG_DATA];
            //var gridData = data[Constants.JSON_TAG_GRIDDATA];

            //var o = JsonConvert.DeserializeObject<JObject>(gridData.ToString());
            //var listObj = o.Value<JArray>("rowset").ToObject<List<ReceptionDTO>>();

            var o = JsonConvert.DeserializeObject<JObject>(myJsonString);
            var h = o.Value<JObject>("fs_DATABROWSE_V554211")
                     .Value<JObject>("data")
                     .Value<JArray>("rowset")
                     .ToObject<List<ReceptionDTO>>();

            //SetValues(rowset, diccionary, columnId, columnValue);
        }

        private void SetValues(JToken rowset, Dictionary<String, tblRecepcion> diccionary, String columnId, String columnValue)
        {
            while (rowset.First != null)
            {
                String id = rowset.First[columnId].ToString();
                String value = rowset.First[columnValue].ToString();
                AddDataToDictionary(diccionary, id, value);
                rowset.First.Remove();
            }
        }

        private void AddDataToDictionary(Dictionary<String, tblRecepcion> dictionary, String id, String data)
        {
            tblRecepcion recep = null;
            dictionary.TryGetValue(id, out recep);
            if (recep == null)
            {
                recep = new tblRecepcion();
                // AGREGO LA INFO DEL ROWSET
                /* .
                 * .
                 * .
                 */
                dictionary.Add(id, recep);
            }
        }
    }
}
