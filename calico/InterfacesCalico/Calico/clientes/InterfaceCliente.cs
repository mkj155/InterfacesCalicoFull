using Calico.clientes;
using Calico.common;
using Calico.Persistencia;
using System;
using Nini.Config;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Net;
using Calico.common.mapping;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace InterfacesCalico.clientes
{
    public class InterfaceCliente : InterfaceGeneric
    {
        private BianchiService service = new BianchiService();
        private tblSubClienteService serviceCliente = new tblSubClienteService();
        private const String INTERFACE = Constants.INTERFACE_CLIENTES;
        private ClientesUtils clientesUtils = new ClientesUtils();
        
        public bool process(DateTime? dateTime)
        {
            using (CalicoEntities entities = new CalicoEntities())
            using (DbContextTransaction scope = entities.Database.BeginTransaction())
            {
                    DateTime lastTime;
                    BIANCHI_PROCESS process = service.findByName(INTERFACE);

                    /* Inicializamos los datos del proceso */
                    process.inicio = DateTime.Now;
                    process.maquina = Environment.MachineName;
                    process.process_id = Process.GetCurrentProcess().Id;

                    /* Trata de ejecutar un update a la fila de la interface, si la row se encuentra bloqueada,
                       quedara esperando hasta que se desbloquee */
                    Utils.blockRow(process.id, INTERFACE);

                    /* Bloquea la row, para que no pueda ser actualizada por otra interfaz */
                    entities.Database.ExecuteSqlCommand("SELECT * FROM BIANCHI_PROCESS WITH (ROWLOCK, UPDLOCK) where id = " + process.id);

                    /* Obtenemos la fecha */
                    if (dateTime == null){
                        lastTime = Convert.ToDateTime(process.fecha_ultima);
                    }else{
                        lastTime = Convert.ToDateTime(dateTime);
                    }

                    /* Convierto DateTime a String */
                    String lastStringTime = Utils.convertDateTimeInString(lastTime);

                    /* Cargamos archivo con parametros propios para cada interface */
                    IConfigSource source = new IniConfigSource("calico_config.ini");
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
                    int count = 0;

                    foreach (KeyValuePair<string, tblSubCliente> entry in diccionary)
                    {
                        // Me está devolviendo el mismo ID, falta verificar porque ¿?
                        int sub_proc_id = serviceCliente.callProcedure(tipoProceso, tipoMensaje);
                        // Si hacemos los insert pincha por constrains de PK ya que el ID devuelto por el SP siempre retorna lo mismo
                        entry.Value.subc_proc_id = sub_proc_id;

                        // VERY_HARDCODE
                        // Me los pidio como valores obligatorios.
                        //entry.Value.subc_iva = "21";
                        //entry.Value.subc_codigo = entry.Value.subc_codigoCliente;
                        //entry.Value.subc_domicilio = "Peron 2579";
                        //entry.Value.subc_localidad = "San Vicente";
                        //entry.Value.subc_codigoPostal = "1642";
                        //entry.Value.subc_areaMuelle = "Area17";
                        //entry.Value.subc_telefono = "1512349876";
                        //serviceCliente.save(entry.Value);
                        count++;
                    }

                    // Agregamos datos faltantes de la tabla de procesos
                    process.fin = DateTime.Now;
                    process.cant_lineas = count;
                    process.estado = Constants.ESTADO_OK;

                    /* Liberamos la row, para que la tome otra interface */
                    scope.Commit();
              
                    /* Actualizamos la tabla BIANCHI_PROCESS */
                    service.update(process);

                    return true;
                
            }
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
                cliente.subc_codigoCliente = id;
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

    }
}
