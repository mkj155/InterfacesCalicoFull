using Calico.clientes;
using Calico.common;
using Calico.Persistencia;
using System;
using Nini.Config;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;

namespace InterfacesCalico.clientes
{
    public class InterfaceCliente : InterfaceGeneric
    {
        private const String INTERFACE = Constants.INTERFACE_CLIENTES;

        private BianchiService service = new BianchiService();
        private tblSubClienteService serviceCliente = new tblSubClienteService();
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
                Console.WriteLine("Inicializando los datos del proceso");
                process.inicio = DateTime.Now;
                process.maquina = Environment.MachineName;
                process.process_id = Process.GetCurrentProcess().Id;
                Console.WriteLine("Inicio: " + process.inicio);
                Console.WriteLine("Maquina: " + process.maquina);
                Console.WriteLine("Process_id: " + process.process_id);

                /* Trata de ejecutar un update a la fila de la interface, si la row se encuentra bloqueada, quedara esperando hasta que se desbloquee */
                Console.WriteLine("Verificamos que no haya otro proceso corriendo para la misma interfaz: " + INTERFACE);
                service.blockRow(process.id, INTERFACE);

                /* Bloquea la row, para que no pueda ser actualizada por otra interfaz */
                Console.WriteLine("Bloqueamos la row de BIANCHI_PROCESS, para la interfaz " + INTERFACE);
                entities.Database.ExecuteSqlCommand("SELECT * FROM BIANCHI_PROCESS WITH (ROWLOCK, UPDLOCK) where id = " + process.id);

                /* Obtenemos la fecha */
                if (dateTime == null && process.fecha_ultima == null)
                {
                    Console.WriteLine("La fecha de BIANCHI_PROCESS es NULL y no se indico fecha como parametro, no se ejecutara el proceso para la interfaz :" + INTERFACE);
                    Console.WriteLine("Se libera la row de BIANCHI_PROCESS");
                    scope.Commit();
                    return false;
                }
                else if (dateTime == null)
                {
                    Console.WriteLine("Se procesará la interfaz: " + INTERFACE + " con la fecha de BIANCHI_PROCESS: " + process.fecha_ultima);
                    lastTime = Convert.ToDateTime(process.fecha_ultima);
                }
                else
                {
                    Console.WriteLine("Se procesará la interfaz: " + INTERFACE + " con la fecha pasada como argumentos: " + dateTime);
                    lastTime = Convert.ToDateTime(dateTime);
                }

                /* Convierto DateTime a String */
                String lastStringTime = Utils.convertDateTimeInString(lastTime);

                /* Cargamos archivo con parametros propios para cada interface */
                Console.WriteLine("Cargamos archivo de configuracion");
                IConfigSource source = new IniConfigSource("calico_config.ini");

                /* Obtenemos las keys de las URLs del archivo externo */
                String[] URLkeys = source.Configs[INTERFACE + "." + Constants.URLS].GetKeys();

                /* Preparamos la URL con sus parametros y llamamos al servicio */
                String urlPath = String.Empty;
                String user = source.Configs[Constants.BASIC_AUTH].Get(Constants.USER);
                String pass = source.Configs[Constants.BASIC_AUTH].Get(Constants.PASS);
                Console.WriteLine("Usuario del Servicio Rest: " + user);

                /* Obtenemos las URLs, las armamos con sus parametros, obtenemos los datos y armamos los objetos Clientes */
                Dictionary<String, tblSubCliente> diccionary = new Dictionary<string, tblSubCliente>();
                foreach (String key in URLkeys)
                {
                    // Obtenemos las URLs
                    String url = source.Configs[INTERFACE + "." + Constants.URLS].Get(key);
                    // Armamos la URL
                    urlPath = clientesUtils.buildUrl(url, key, lastStringTime);
                    Console.WriteLine("Url: " + urlPath);
                    // Obtenemos los datos
                    String myJsonString = Utils.sendRequest(urlPath, user, pass, key, diccionary);
                    // Armamos los objetos Clientes
                    clientesUtils.mappingCliente(myJsonString, key, diccionary);
                }

                // LLamando al SP por cada cliente
                int? tipoProceso = source.Configs[INTERFACE].GetInt(Constants.NUMERO_INTERFACE_CLIENTE);
                int? tipoMensaje = 0;
                int count = 0;
                int countError = 0;
                Console.WriteLine("Codigo de interface: " + tipoProceso);
                Console.WriteLine("Llamando al SP por cada cliente");
                foreach (KeyValuePair<string, tblSubCliente> entry in diccionary)
                {
                    int sub_proc_id = serviceCliente.callProcedure(tipoProceso, tipoMensaje);
                    entry.Value.subc_proc_id = sub_proc_id;

                    // VERY_HARDCODE
                    // Los pidio como valores obligatorios.
                    entry.Value.subc_iva = "21";
                    entry.Value.subc_codigo = entry.Value.subc_codigoCliente;
                    entry.Value.subc_domicilio = "Peron 2579";
                    entry.Value.subc_localidad = "San Vicente";
                    entry.Value.subc_codigoPostal = "1642";
                    entry.Value.subc_areaMuelle = "Area17";
                    entry.Value.subc_telefono = "1512349876";
                    try
                    {
                        serviceCliente.save(entry.Value);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Error al agregar cliente: " + entry.Value.subc_codigoCliente);
                        Console.Error.WriteLine(ex.Message);
                        countError++;
                    }
                    count++;
                }

                Console.WriteLine("Finalizó el proceso de actualización de clientes");
                Console.WriteLine(countError + " Clientes no pudieron ser procesados");

                /* Agregamos datos faltantes de la tabla de procesos */
                Console.WriteLine("Preparamos la actualizamos de BIANCHI_PROCESS");
                process.fin = DateTime.Now;
                process.cant_lineas = count;
                process.estado = Constants.ESTADO_OK;
                Console.WriteLine("Fecha_fin: " + process.fin);
                Console.WriteLine("Cantidad de clientes procesados: " + process.cant_lineas);
                Console.WriteLine("Estado: " + process.estado);

                /* Liberamos la row, para que la tome otra interface */
                Console.WriteLine("Se libera la row de BIANCHI_PROCESS");
                scope.Commit();

                /* Actualizamos la tabla BIANCHI_PROCESS */
                Console.WriteLine("Actualizamos BIANCHI_PROCESS");
                service.update(process);

                Console.WriteLine("Fin del proceso, para la interfaz " + INTERFACE);
                Console.WriteLine("Proceso Finalizado correctamente");

                return true;
                
            }
        }

    }
}
