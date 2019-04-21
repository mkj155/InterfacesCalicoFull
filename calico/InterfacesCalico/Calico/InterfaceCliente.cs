using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace InterfacesCalico
{
    public class InterfaceCliente : InterfaceGeneric
    {
        public void sendRequest(String url, List<String> parameters)
        {
            StringBuilder concat = new StringBuilder();
            int count = 0;
            foreach (String param in parameters) {
                if(count > 0) concat.Append("/");
                concat.Append(param);
            }
            HttpWebRequest request = WebRequest.Create(url + concat) as HttpWebRequest;
            request.Method = "GET";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string body = reader.ReadToEnd();
            System.Console.WriteLine(body);
        }

    }
}
