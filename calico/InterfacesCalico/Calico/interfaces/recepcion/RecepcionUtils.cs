using Calico.common;
using Calico.persistencia;
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
        public String BuildUrl(String urlParam, String key, String fecha)
        {
            String url = String.Empty;
            //....
            return url;
        }

        private String GetHeaderJson(String key)
        {
            String header = String.Empty;
            //....
            return header;
        }

        public void MappingRecepcion(String myJsonString, String key, Dictionary<String, tblRecepcion> diccionary)
        {
            var json = JObject.Parse(myJsonString);
            var root = json[GetHeaderJson(key)];
            var data = root[Constants.JSON_TAG_DATA];
            var gridData = data[Constants.JSON_TAG_GRIDDATA];
            var rowset = gridData[Constants.JSON_TAG_ROWSET];

            //....
        }

        private void SetValues(JToken rowset, String key, Dictionary<String, tblRecepcion> diccionary, String columnId, String columnValue)
        {
            while (rowset.First != null)
            {
                String id = rowset.First[columnId].ToString();
                String value = rowset.First[columnValue].ToString();
                AddDataToDictionary(diccionary, id, value, key);
                rowset.First.Remove();
            }
        }

        private void AddDataToDictionary(Dictionary<String, tblRecepcion> dictionary, String id, String data, String key)
        {
            //....
        }
    }
}
