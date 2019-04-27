using Calico;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace InterfacesCalico
{
    public class InterfaceCliente : InterfaceGeneric
    {
        BianchiService service = new BianchiService();
        public const String NAME_INTERFACE = "Clientes";

        public string process(String url, List<String> parameters,DateTime dateLast)
        {
            /* Inicio el proceso con process_id,name machine,fecha init,etc */
            BIANCHI_PROCESS process = service.getProcessInit(dateLast, NAME_INTERFACE);
            /* process interface */
            sendRequest(url, parameters);

            /* Update fields process */
            process.fin = DateTime.Now;
            process.cant_lineas = 20;
            process.estado = "ok";
            service.save(process);

            return "ok";
        }

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
