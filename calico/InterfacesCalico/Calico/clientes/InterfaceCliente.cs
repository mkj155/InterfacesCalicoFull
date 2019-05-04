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
                    Console.WriteLine("Comienzo del proceso para la interfaz " + INTERFACE);

                    DateTime lastTime;
                    BIANCHI_PROCESS process = service.findByName(INTERFACE);

                    /* Inicializamos los datos del proceso */
                    Console.WriteLine("Inicializamos los datos del proceso");
                    Console.WriteLine("Inicio : " + DateTime.Now);
                    Console.WriteLine("Maquina: " + Environment.MachineName);
                    Console.WriteLine("Process id: " + Process.GetCurrentProcess().Id);
                    process.inicio = DateTime.Now;
                    process.maquina = Environment.MachineName;
                    process.process_id = Process.GetCurrentProcess().Id;

                    /* Trata de ejecutar un update a la fila de la interface, si la row se encuentra bloqueada,
                    quedara esperando hasta que se desbloquee */
                    Console.WriteLine("Se verifica que no haya otro proceso corriendo para la misma interfaz " + INTERFACE);
                    Console.WriteLine("En caso de que haya, el proceso se bloqueara...");
                    service.blockRow(process.id, INTERFACE);

                    /* Bloquea la row, para que no pueda ser actualizada por otra interfaz */
                    Console.WriteLine("Se bloquea la row de la tabla Bianchi_process, para la interfaz " + INTERFACE);
                    entities.Database.ExecuteSqlCommand("SELECT * FROM BIANCHI_PROCESS WITH (ROWLOCK, UPDLOCK) where id = " + process.id);

                    /* Obtenemos la fecha */
                    if (dateTime == null && process.fecha_ultima == null){
                        Console.WriteLine("La fecha de la tabla bianchi_process es nula y " +
                            "no se indico fecha como parametro, no se ejecutara el proceso");
                        Console.WriteLine("Se libera la row de la tabla procesos");
                        scope.Commit();
                        return false;
                    }else if(dateTime == null){
                        Console.WriteLine("Se obtiene la ultima fecha enviada," +
                            "desde Bianchi_process: " + process.fecha_ultima);
                        lastTime = Convert.ToDateTime(process.fecha_ultima);
                    }else {
                        Console.WriteLine("Se obtiene la fecha desde los argumentos: " + dateTime);
                        lastTime = Convert.ToDateTime(dateTime);
                    }

                    /* Convierto DateTime a String */
                    String lastStringTime = Utils.convertDateTimeInString(lastTime);

                    /* Cargamos archivo con parametros propios para cada interface */
                    IConfigSource source = new IniConfigSource("calico_config.ini");
                    /* Obtenemos las keys de las URLs del archivo externo */
                    Console.WriteLine("Se obtiene la configuración desde el archivo calico_config.ini");
                    String[] URLkeys = source.Configs[INTERFACE + "." + Constants.URLS].GetKeys();

                    /* Preparamos la URL con sus parametros y llamamos al servicio */
                    Console.WriteLine("Se obtiene la configuración desde el archivo calico_config.ini");
                    String urlPath = String.Empty;
                    String user = source.Configs[Constants.BASIC_AUTH].Get(Constants.USER);
                    String pass = source.Configs[Constants.BASIC_AUTH].Get(Constants.PASS);
                    Console.WriteLine("User: " + user);
                    Console.WriteLine("Password: " + pass);

                    /* Obtenemos las URLs, las armamos con sus parametros, obtenemos los datos y armamos los objetos Clientes */
                    Dictionary<String, tblSubCliente> diccionary = new Dictionary<string, tblSubCliente>();
                    foreach (String key in URLkeys)
                    {
                        // Obtenemos las URLs
                        String url = source.Configs[INTERFACE + "." + Constants.URLS].Get(key);
                        Console.WriteLine("Url: " + url);
                        // Armamos la URL
                        urlPath = clientesUtils.buildUrl(url, key, lastStringTime);
                        Console.WriteLine("Armamos la url con los parametros: " + urlPath);
                        // obtenemos los datos y armamos los objetos Clientes
                        Console.WriteLine("Llamada al servicio,obtenemos los datos y armamos los objetos " + INTERFACE);
                        sendRequest(urlPath, user, pass, key, diccionary);
                    }

                    // TODO AGREGAR LLAMADO A SU SP NumeroInterface
                    int? tipoProceso = source.Configs[INTERFACE].GetInt(Constants.NUMERO_INTERFACE_CLIENTE);
                    int? tipoMensaje = 0;
                    int count = 0;
                    Console.WriteLine("Obtenemos el numero de interface desde el archivo de configuración: " + tipoProceso);

                Console.WriteLine("Comienzo del proceso para completar el mapa de " + INTERFACE + ", y guardarlos en la base de datos");
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
                    Console.WriteLine("Finalizó el guardado de los " + INTERFACE + " en la base de datos");

                    /* Agregamos datos faltantes de la tabla de procesos */
                    Console.WriteLine("Se agregan los datos faltantes a la tabla de procesos");
                    Console.WriteLine("Fecha_fin: " + DateTime.Now);
                    Console.WriteLine("Cantidad de lineas procesadas: " + count);
                    Console.WriteLine("Estado: " + Constants.ESTADO_OK);
                    process.fin = DateTime.Now;
                    process.cant_lineas = count;
                    process.estado = Constants.ESTADO_OK;

                    /* Liberamos la row, para que la tome otra interface */
                    Console.WriteLine("Se libera la row de la tabla procesos");
                    scope.Commit();
              
                    /* Actualizamos la tabla BIANCHI_PROCESS */
                    service.update(process);

                    Console.WriteLine("Fin del proceso, para la interfaz " + INTERFACE);

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
