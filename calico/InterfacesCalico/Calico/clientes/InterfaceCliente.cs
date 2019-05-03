using Calico.clientes;
using Calico.common;
using Calico.Persistencia;
using Nini.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Calico.common.mapping;
using Newtonsoft.Json.Linq;
using System.Text;

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
            String[] URLkeys = source.Configs[INTERFACE + "." + Constants.URLS].GetKeys();

            // Preparamos la URL con sus parametros y llamamos al servicio
            String urlPath = String.Empty;
            String user = source.Configs[Constants.BASIC_AUTH].Get(Constants.USER);
            String pass = source.Configs[Constants.BASIC_AUTH].Get(Constants.PASS);

            // Obtenemos las URLs, las armamos con sus parametros, obtenemos los datos y armamos los objetos Clientes
            Dictionary<String, tblSubCliente> diccionary = new Dictionary<string, tblSubCliente>();
            foreach (String key in URLkeys)
            {
                // Obtenemos las URLs
                String url = source.Configs[INTERFACE + "." + Constants.URLS].Get(key);
                // Armamos la URL
                urlPath = clientesUtils.buildUrl(url, key, lastStringTime);
                // obtenemos los datos y armamos los objetos Clientes
                sendRequest(urlPath, user, pass, key, diccionary);
            }

            // TODO AGREGAR LLAMADO A SU SP NumeroInterface
            int? tipoProceso = source.Configs[INTERFACE].GetInt(Constants.NUMERO_INTERFACE_CLIENTE);
            int? tipoMensaje = 0;
            serviceCliente.callProcedure(tipoProceso, tipoMensaje);

            // Actualizamos el cliente
            // VAMOS A EJECUTAR UNA PRUEBA DE CREACION, ACTUALIZACION Y BORRADO DE UN CLIENTE SOLO PARA VER FUNCIONALIDAD
            // serviceCliente.examplePersist();

            // Agregamos datos faltantes
            process.fin = DateTime.Now;
            process.cant_lineas = 20;
            process.estado = Constants.ESTADO_OK;

            // Actualizamos la tabla BIANCHI_PROCESS
            // service.save(process);
            service.update(process);

            return true;
        }

        public void sendRequest(string url, String user, String pass, String key, Dictionary<String, tblSubCliente> diccionary)
        {
            List<Rowset> rowsetList = new List<Rowset>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + pass));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Method = Constants.METHOD_GET;
            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string myJsonString = reader.ReadToEnd();
                simpleMapping(myJsonString, key, diccionary);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private String getHeaderJson(String key)
        {
            String header = String.Empty;

            if (Constants.MLNM.Equals(key))
            {
                header = Constants.JSON_PREFIX + Constants.JSON_SUBFIX_MLNM;
            }
            else if (Constants.TAX.Equals(key))
            {
                header = Constants.JSON_PREFIX + Constants.JSON_SUBFIX_TAX;
            }

            return header;
        }

        private void addDataToDictionary(Dictionary<String, tblSubCliente> dictionary, String id, String data, String key)
        {
            tblSubCliente cliente = null;
            dictionary.TryGetValue(id, out cliente);
            if (cliente == null)
            {
                cliente = new tblSubCliente();
                dictionary.Add(id, cliente);
            }
            if (Constants.MLNM.Equals(key))
            {
                cliente.subc_razonSocial = data;
            }
            else if (Constants.TAX.Equals(key))
            {
                cliente.subc_cuit = data;
            }
        }

        private void simpleMapping(String myJsonString, String key, Dictionary<String, tblSubCliente> diccionary)
        {
            var json = JObject.Parse(myJsonString);
            var root = json[getHeaderJson(key)];
            var data = root[Constants.JSON_TAG_DATA];
            var gridData = data[Constants.JSON_TAG_GRIDDATA];
            var rowset = gridData[Constants.JSON_TAG_ROWSET];

            String AN8 = String.Empty;
            String value = String.Empty;

            if (Constants.MLNM.Equals(key))
            {
                while (rowset.First != null)
                {
                    AN8 = rowset.First[Constants.JSON_SUBFIX_MLNM + "_" + Constants.COLUMN_AN8].ToString();
                    value = rowset.First[Constants.JSON_SUBFIX_MLNM + "_" + Constants.COLUMN_MLNM].ToString();
                    addDataToDictionary(diccionary, AN8, value, key);
                    rowset.First.Remove();
                }
            }
            else if (Constants.TAX.Equals(key))
            {
                while (rowset.First != null)
                {
                    AN8 = rowset.First[Constants.JSON_SUBFIX_TAX + "_" + Constants.COLUMN_AN8].ToString();
                    value = rowset.First[Constants.JSON_SUBFIX_TAX + "_" + Constants.COLUMN_TAX].ToString();
                    addDataToDictionary(diccionary, AN8, value, key);
                    rowset.First.Remove();
                }
            }
        }

        //private void iterateRowset(JToken rowset, String key, Dictionary<String, tblSubCliente> diccionary)
        //{
        //    String AN8 = String.Empty;
        //    String value = String.Empty;

        //    while (rowset.First != null)
        //    {
        //        AN8 = rowset.First[Constants.JSON_SUBFIX_MLNM + "_" + Constants.COLUMN_AN8].ToString();
        //        value = rowset.First[Constants.JSON_SUBFIX_MLNM + "_" + Constants.COLUMN_MLNM].ToString();
        //        addDataToDictionary(diccionary, AN8, value, key);
        //        rowset.First.Remove();
        //    }
        //}
            //while (rowset.First != null)
            //{
            //    if (Constants.MLNM.Equals(key))
            //    {
            //        AN8 = rowset.First[Constants.JSON_SUBFIX_MLNM + "_" + Constants.COLUMN_AN8].ToString();
            //        value = rowset.First[Constants.JSON_SUBFIX_MLNM + "_" + Constants.COLUMN_MLNM].ToString();
            //    }
            //    else if (Constants.TAX.Equals(key))
            //    {
            //        AN8 = rowset.First[Constants.JSON_SUBFIX_TAX + "_" + Constants.COLUMN_AN8].ToString();
            //        value = rowset.First[Constants.JSON_SUBFIX_TAX + "_" + Constants.COLUMN_TAX].ToString();
            //    }
            //    addDataToDictionary(diccionary, AN8, value, key);
            //    rowset.First.Remove();
            //}
        

    }
}
