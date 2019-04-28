using Calico;
using Calico.clientes;
using Calico.common;
using Calico.Persistencia;
using Nini.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace InterfacesCalico.clientes
{
    public class InterfaceCliente : InterfaceGeneric
    {
        private BianchiService service = new BianchiService();
        private tblSubClienteService serviceCliente = new tblSubClienteService();
        public const String INTERFACE = Constants.INTERFACE_CLIENTES;
        
        public bool process(IConfigSource source, DateTime? dateTime)
        {
            // Inicio del proceso (inicializacion del objeto de la Tabla BIANCHI_PROCESS)
            BIANCHI_PROCESS process = service.getProcessInit(dateTime, INTERFACE);

            // Obtenemos la url del archivo externo
            string urlPath = source.Configs[INTERFACE].Get("url");

            // Preparamos los parametros
            List<String> parameters = new List<string>();
            if (dateTime != null)
            {
                String param = Regex.Replace(dateTime.ToString(), @"\s+", "%20");
                // Reemplazamos por el momento la fecha por un String (HARDCODE pasar parametro correspondiente)
                parameters.Add("20190503");
            }

            // Obtenemos los datos
            sendRequest(urlPath, parameters);

            // ACA DEBERIAMOS PROCESAR LA INTERFACE
            // CREACION DE CLIENTES
            // VALIDACIONES VARIAS SOBRE LOS DATOS CLIENTES
            // ETC...

            // llamamos a un STORE PROCEDURE de prueba
            serviceCliente.callProcedure("Nombre");

            // Actualizamos el cliente
            // VAMOS A EJECUTAR UNA PRUEBA DE CREACION, ACTUALIZACION Y BORRADO DE UN CLIENTE SOLO PARA VER FUNCIONALIDAD
            serviceCliente.examplePersist();

            // Agregamos datos faltantes
            process.fin = DateTime.Now;
            process.cant_lineas = 20;
            process.estado = "OK";

            // Actualizamos la tabla BIANCHI_PROCESS
            service.save(process);

            return true;
        }

        public void sendRequest(String url, List<String> parameters)
        {
            StringBuilder concat = new StringBuilder();
            int count = 0;
            foreach (String param in parameters)
            {
                if (count > 0) concat.Append("/");
                concat.Append(param);
                count++;
            }
            HttpWebRequest request = WebRequest.Create(url + concat) as HttpWebRequest;
            request.Method = Constants.METHOD_GET;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string body = reader.ReadToEnd();
            System.Console.WriteLine(body);
        }

    }
}
