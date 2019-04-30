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
        private const String INTERFACE = Constants.INTERFACE_CLIENTES;
        private ClientesUtils clientesUtils = new ClientesUtils();
        
        public bool process(IConfigSource source, DateTime? dateTime)
        {
            // Obtenemos la fecha
            DateTime lastTime;
            if (dateTime == null) {
                lastTime = Convert.ToDateTime(service.getProcessDate(INTERFACE));
            } else {
                lastTime = Convert.ToDateTime(dateTime);
            }

            bool existProcess = false;
            // Si el estado es "EN_CURSO" cancelamos la ejecucion
            if (/*!service.validarSiPuedoProcesar(INTERFACE)*/ false) {
                Console.WriteLine("La interface " + INTERFACE + " se esta ejecutando actualmente.");
                return false;
            }

            // Convierto DateTime a String
            String lastStringTime = Utils.convertDateTimeInString(lastTime);

            // Si esta OK para ejecutar tomamos control del proceso y actualizamos la tabla BIANCHI_PROCESS
            existProcess = (service.updateEnCurso(INTERFACE));

            // Inicio del proceso (inicializacion del objeto de la Tabla BIANCHI_PROCESS)
            BIANCHI_PROCESS process = service.getProcessInit(dateTime, INTERFACE);

            // Si nunca se ejecuto insertamos el registro con estado "EN_CURSO"
            if (!existProcess)
            {
                process.estado = Constants.ESTADO_EN_CURSO;
                service.save(process);
            }

            // Obtenemos las keys de las URLs del archivo externo
            String[] URLkeys = source.Configs[INTERFACE+"."+Constants.URLS].GetKeys();

            // Preparamos la URL con sus parametros y llamamos al servicio
            String urlPath = String.Empty;
            foreach (String key in URLkeys)
            {
                String url = source.Configs[INTERFACE + "." + Constants.URLS].Get(key);
                urlPath = clientesUtils.buildUrl(url, key, lastStringTime);
                // Obtenemos los datos
                sendRequest(urlPath);
            }

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
            process.estado = Constants.ESTADO_OK;

            // Actualizamos la tabla BIANCHI_PROCESS
            // service.save(process);
            service.update(process);

            return true;
        }

        public void sendRequest(String url)
        {
            //var webRequest = WebRequest.Create(url);
            //webRequest.Credentials = new NetworkCredential("userName", "password");
            //using (var webResponse = webRequest.GetResponse())
            //{
            //    using (var responseStream = webResponse.GetResponseStream())
            //    {
            //        new StreamReader(responseStream).ReadToEnd();
            //    }
            //}
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Credentials = new NetworkCredential("CALICO", "C4l1c02020");
            request.Method = Constants.METHOD_GET;
            /**/
            //Uri myUri = new Uri(url);
            //WebRequest myWebRequest = WebRequest.Create(myUri);
            //HttpWebRequest myHttpWebRequest = (HttpWebRequest)myWebRequest;
            //NetworkCredential myNetworkCredential = new NetworkCredential("CALICO", "C4l1c02020");
            //CredentialCache myCredentialCache = new CredentialCache();
            //myCredentialCache.Add(myUri, "Basic", myNetworkCredential);
            //myHttpWebRequest.PreAuthenticate = true;
            //myHttpWebRequest.Credentials = myCredentialCache;
            //WebResponse myWebResponse = myWebRequest.GetResponse();
            //Stream responseStream = myWebResponse.GetResponseStream();
            //StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);
            //string pageContent = myStreamReader.ReadToEnd();
            //responseStream.Close();
            //myWebResponse.Close();
            /**/
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string body = reader.ReadToEnd();
            System.Console.WriteLine(body);
        }

    }
}
