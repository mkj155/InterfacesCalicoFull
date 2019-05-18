using Calico.common;
using Calico.persistencia;
using Calico.service;
using InterfacesCalico.generic;
using Nini.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.interfaces.recepcion
{
    class InterfaceRecepcion : InterfaceGeneric
    {
        private const String INTERFACE = Constants.INTERFACE_RECEPCION;

        private BianchiService service = new BianchiService();
        private TblRecepcionService serviceRecepcion = new TblRecepcionService();
        private RecepcionUtils recepcionUtils = new RecepcionUtils();

        public bool Process(DateTime? dateTime)
        {
            Console.WriteLine("Comienzo del proceso para la interfaz " + INTERFACE);

            BIANCHI_PROCESS process = service.FindByName(INTERFACE);
            if (process == null)
            {
                Console.WriteLine("No hay configuracion en BIANCHI_PROCESS para la interface: " + INTERFACE);
                Console.WriteLine("Finalizamos la ejecucion de la interface: " + INTERFACE);
                return false;
            }

            /* Inicializamos los datos del proceso */
            Console.WriteLine("Inicializando los datos del proceso");
            process.inicio = DateTime.Now;
            process.maquina = Environment.MachineName;
            process.process_id = System.Diagnostics.Process.GetCurrentProcess().Id;
            Console.WriteLine("Inicio: " + process.inicio);
            Console.WriteLine("Maquina: " + process.maquina);
            Console.WriteLine("Process_id: " + process.process_id);

            /* Bloquea la row, para que no pueda ser actualizada por otra ejecucion de la misma interface */
            Console.WriteLine("Si hay otro proceso ejecutandose para la interface " + INTERFACE + " esperamos a que termine");
            Console.WriteLine("Bloqueando la row de BIANCHI_PROCESS, para la interfaz " + INTERFACE);
            service.LockRow(process.id);

            /* Obtenemos la fecha */
            if (Utils.IsInvalidateDates(dateTime, process.fecha_ultima))
            {
                service.UnlockRow();
                return false;
            }
            DateTime lastTime = Utils.GetDateToProcess(dateTime, process.fecha_ultima);

            /* Convierto DateTime a String */
            String lastStringTime = lastStringTime = Utils.ConvertDateTimeInString(lastTime);

            /* Cargamos archivo con parametros propios para cada interface */
            Console.WriteLine("Cargamos archivo de configuracion");
            IConfigSource source = new IniConfigSource("calico_config.ini");

            /* Obtenemos usuario y contraseña del archivo para el servicio Rest */
            String urlPath = String.Empty;
            String user = source.Configs[Constants.BASIC_AUTH].Get(Constants.USER);
            String pass = source.Configs[Constants.BASIC_AUTH].Get(Constants.PASS);
            Console.WriteLine("Usuario del Servicio Rest: " + user);
            
            /* Obtenemos la URL del archivo */
            String url = source.Configs[INTERFACE + "." + Constants.URLS].GetString(Constants.INTERFACE_RECEPCION_URL);

            /* Armamos la URL con parametros */
            urlPath = recepcionUtils.BuildUrl(url, lastStringTime);
            Console.WriteLine("URL: " + urlPath);

            /* Obtenemos los datos */
            String myJsonString = Utils.SendRequest(urlPath, user, pass);

            /* Mapping */
            List<ReceptionDTO> receptionDTO = null;
            Dictionary<String, tblRecepcion> dictionary = new Dictionary<string, tblRecepcion>();
            String emplazamiento = source.Configs[Constants.INTERFACE_RECEPCION].GetString(Constants.INTERFACE_RECEPCION_EMPLAZAMIENTO);
            String almacen = source.Configs[Constants.INTERFACE_RECEPCION].GetString(Constants.INTERFACE_RECEPCION_ALMACEN);
            String tipo = source.Configs[Constants.INTERFACE_RECEPCION].GetString(Constants.INTERFACE_RECEPCION_CODIGO);
            String compania = source.Configs[Constants.INTERFACE_RECEPCION].GetString(Constants.INTERFACE_RECEPCION_COMPANIA);

            if (!String.Empty.Equals(myJsonString))
            {
                receptionDTO = recepcionUtils.MappingJsonRecepcion(myJsonString);
                if (receptionDTO.Any())
                {
                    recepcionUtils.MappingReceptionDTORecepcion(receptionDTO, dictionary, emplazamiento, almacen, tipo, compania);
                }
                else
                {
                    Utils.finishProcessByError(service, process, Constants.NOT_DATA_FOUND, INTERFACE);
                    return false;
                }
            }
            else
            {
                Utils.finishProcessByError(service, process, Constants.FAILED_CALL_REST, INTERFACE);
                return false;
            }

            int count = 0;
            int countError = 0;
            int countAlreadyProcess = 0;
            int? tipoMensaje = 0;
            int? tipoProceso = source.Configs[INTERFACE].GetInt(Constants.NUMERO_INTERFACE);
            int codigoCliente = source.Configs[INTERFACE].GetInt(Constants.NUMERO_CLIENTE_INTERFACE_RECEPCION);
            Console.WriteLine("Codigo de interface: " + tipoProceso);

            // Validamos si hay que insertar o descartar la recepcion
            foreach (KeyValuePair<string, tblRecepcion> entry in dictionary)
            {
                // ¿Ya está procesada?
                if (serviceRecepcion.IsAlreadyProcess(emplazamiento, almacen, tipo, entry.Value.recc_numero))
                {
                    Console.WriteLine("La recepcion " + entry.Value.recc_numero + " ya fue tratada, no se procesara");
                    countAlreadyProcess++;
                }
                // No está procesada! la voy a guardar
                else
                {
                    // LLamo al SP y seteo su valor a la cabecera y sus detalles
                    int recc_proc_id = serviceRecepcion.CallProcedure(tipoProceso, tipoMensaje);
                    entry.Value.recc_proc_id = recc_proc_id;
                    foreach (tblRecepcionDetalle detalle in entry.Value.tblRecepcionDetalle)
                    {
                        detalle.recd_proc_id = recc_proc_id;
                    }
                    // ¿La pude guardar?
                    Console.WriteLine("Procesando recepcion: " + entry.Value.recc_numero);
                    if (serviceRecepcion.Save(entry.Value))
                        count++;
                    else
                        countError++;
                }
            }

            Console.WriteLine("Finalizó el proceso de actualización de Recepciones");

            /* Agregamos datos faltantes de la tabla de procesos */
            Console.WriteLine("Preparamos la actualizamos de BIANCHI_PROCESS");
            process.fin = DateTime.Now;
            process.cant_lineas = count;
            process.estado = Constants.ESTADO_OK;
            Console.WriteLine("Fecha_fin: " + process.fin);
            Console.WriteLine("Cantidad de Recepciones procesadas OK: " + process.cant_lineas);
            Console.WriteLine("Cantidad de Recepciones procesadas con ERROR: " + countError);
            Console.WriteLine("Cantidad de Recepciones evitadas: " + countAlreadyProcess);
            Console.WriteLine("Estado: " + process.estado);

            /* Actualizamos la tabla BIANCHI_PROCESS */
            Console.WriteLine("Actualizamos BIANCHI_PROCESS");
            service.Update(process);

            /* Liberamos la row, para que la tome otra interface */
            Console.WriteLine("Se libera la row de BIANCHI_PROCESS");
            service.UnlockRow();

            Console.WriteLine("Fin del proceso, para la interfaz " + INTERFACE);
            Console.WriteLine("Proceso Finalizado correctamente");

            return true;
        }
    }
}
