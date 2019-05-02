using Calico;
using Calico.clientes;
using Calico.common;
using Calico.Persistencia;
using Nini.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Calico.common.mapping;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            // SE ESTA CAMBIANDO LA VALIDACION POR LOCKEO, FALTA CODEO Y TEST
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
            String user = source.Configs[Constants.BASIC_AUTH].Get(Constants.USER);
            String pass = source.Configs[Constants.BASIC_AUTH].Get(Constants.PASS);

            // VERY HARDCODE NO FUNCIONABA URL CALICO
            List<Rowset> rowsetList = sendRequest("http://localhost:8080/calico/rest/message/", user, pass);
            //foreach (String key in URLkeys)
            //{
            //    String url = source.Configs[INTERFACE + "." + Constants.URLS].Get(key);
            //    urlPath = clientesUtils.buildUrl(url, key, lastStringTime);
            //    // Obtenemos los datos
            //    sendRequest(urlPath, user, pass);
            //}

            // ACA DEBERIAMOS PROCESAR LA INTERFACE
            // MAPEAR CLIENTE CON ROWSET

            // llamamos a un STORE PROCEDURE de prueba
            // TODO AGREGAR LLAMADO A SU SP
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

        public List<Rowset> sendRequest(String url, String user, String pass)
        {
            List<Rowset> rowsetList = new List<Rowset>();
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
            // request.Credentials = new NetworkCredential("CALICO", "C4l1c02020");
            request.Method = Constants.METHOD_GET;
            /**/
            //Uri myUri = new Uri(url);
            //WebRequest myWebRequest = WebRequest.Create(myUri);
            //HttpWebRequest myHttpWebRequest = (HttpWebRequest)myWebRequest;
            //NetworkCredential myNetworkCredential = new NetworkCredential(user, pass);
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
            try {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string myJsonString = reader.ReadToEnd();
                rowsetList = TestSimpleMapping(myJsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return rowsetList;
        }
     
        private List<Rowset> TestSimpleMapping(String myJsonString)
        {
            List<Rowset> rowsetList = new List<Rowset>();

            // var myJsonString = "{\"fs_DATABROWSER_F0101\": {\"title\": \"Data Browser - F0101 [Address Book Master]\",\"data\": {\"gridData\": {\"id\": 59,\"fullGridId\": \"59\",\"columns\": {\"F0101_AN8\": \"Address Number\",\"F0101_ALKY\": \"Long Address\",\"F0101_TAX\": \"Tax ID\",\"F0101_ALPH\": \"Alpha Name\",\"F0101_MCU\": \"Business Unit\",\"F0101_AT1\": \"Sch Typ\",\"F0101_AC01\": \"Cat Code 1\",\"F0101_AC29\": \"CC 29\"},\"rowset\": [{\"F0101_AT1\": \"O\",\"F0101_AN8\": 1,\"F0101_TAX\": \"430788490\",\"F0101_ALKY\": \" \",\"F0101_AC29\": \" \",\"F0101_ALPH\": \"Financial/Distribution Company\",\"F0101_MCU\": \" 1\",\"F0101_AC01\": \" \"},{\"F0101_AT1\": \"O\",\"F0101_AN8\": 9,\"F0101_TAX\": \"238794511\",\"F0101_ALKY\": \" \",\"F0101_AC29\": \" \",\"F0101_ALPH\": \"Multi-Site Target Company\",\"F0101_MCU\": \" 1\",\"F0101_AC01\": \" \"}],\"summary\": {\"records\": 2,\"moreRecords\": false}}},\"errors\": [],\"warnings\": []},\"stackId\": 1,\"stateId\": 1,\"rid\": \"18036bd3bf70bb7a\",\"currentApp\": \"DATABROWSE_F0101\",\"timeStamp\": \"2019-02-08:14.58.31\",\"sysErrors\": [],\"totalMS\": 510,\"renderMS\": 360}";

            var json = JObject.Parse(myJsonString);
            var rootJ = json["fs_DATABROWSER_F0101"];
            var dataJ = rootJ["data"];
            var gridDataJ = dataJ["gridData"];
            var rowsetJ = gridDataJ["rowset"];
            
            while(rowsetJ.First != null)
            {
                Rowset rowset = new Rowset();
                rowset.F0101_AC01 = rowsetJ.First["F0101_AC01"].ToString();
                rowset.F0101_AN8 = rowsetJ.First["F0101_AN8"].ToString();
                rowset.F0101_TAX = rowsetJ.First["F0101_TAX"].ToString();
                rowset.F0101_ALKY = rowsetJ.First["F0101_ALKY"].ToString();
                rowset.F0101_AC29 = rowsetJ.First["F0101_AC29"].ToString();
                rowset.F0101_ALPH = rowsetJ.First["F0101_ALPH"].ToString();
                rowset.F0101_MCU = rowsetJ.First["F0101_MCU"].ToString();
                rowset.F0101_AC01 = rowsetJ.First["F0101_AC01"].ToString();
                rowsetList.Add(rowset);
                rowsetJ.First.Remove();
            }
            return rowsetList;
        }

    }
}
