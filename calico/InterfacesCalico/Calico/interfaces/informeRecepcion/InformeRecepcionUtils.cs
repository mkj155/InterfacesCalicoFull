using Calico.common;
using Calico.interfaces.recepcion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        public static InformeRecepcionJson getObjectTest()
        {
            InformeRecepcionJson jsonObj = new InformeRecepcionJson();
            InformeRecepcionDTO receptDTO = new InformeRecepcionDTO();

            receptDTO.OrderCompany = "00001";
            receptDTO.OrderType = "OT";
            receptDTO.OrderNumber = "849";
            receptDTO.OrderLine = "1.000";
            receptDTO.QuantityToRecieve = "400";
            receptDTO.ReceiptDate = "2019/05/10";
            receptDTO.Lot = "LoteTEST";

            jsonObj.ReceiptsVersion = "BIA0100";
            jsonObj.ReceiptsArray.Add(receptDTO);

            return jsonObj;
        }

        public static Boolean SendRequestPost(string url, String user, String pass, String json)
        {
            String myJsonString = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + pass));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.ContentType = "application/json";
            request.Method = Constants.METHOD_POST;
            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
            }
            catch (WebException e)
            {
                Utils.handleErrorRest(e);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            return false;

        }
    }
}
