using Calico.common;
using Calico.interfaces.recepcion;
using Calico.persistencia;
using Calico.service;
using InterfacesCalico.generic;
using Nini.Config;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.interfaces.informeRecepcion
{
    class InterfaceInformeRecepcion : InterfaceGeneric
    {

        private const String INTERFACE = Constants.INTERFACE_INFORME_RECEPCION;

        private BianchiService service = new BianchiService();
        private TblInformeRecepcionService serviceInformeRecepcion = new TblInformeRecepcionService();
        private InformeRecepcionUtils informeRecepcionUtils = new InformeRecepcionUtils();

        public bool ValidateDate() => false;

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
            if (!service.LockRow(process.id))
            {
                Console.WriteLine("No se pudo lockear la row para la interface " + INTERFACE + " se cancela la ejecucion");
                return false;
            }

            DateTime lastTime = Utils.GetDateToProcess(dateTime, process.fecha_ultima);

            /* Cargamos archivo con parametros propios para cada interface */
            Console.WriteLine("Cargamos archivo de configuracion");
            IConfigSource source = null;
            try
            {
                source = new IniConfigSource("calico_config.ini");
            }
            catch (Exception)
            {
                service.finishProcessByError(process, Constants.FAILED_LOAD_FILE, INTERFACE);
                return false;
            }

            // INICIO BUSQUEDA DE DATOS
            String emplazamiento = source.Configs[Constants.INTERFACE_INFORME_RECEPCION].GetString(Constants.INTERFACE_RECEPCION_EMPLAZAMIENTO);
            String almacen = source.Configs[Constants.INTERFACE_INFORME_RECEPCION].GetString(Constants.INTERFACE_RECEPCION_ALMACEN);
            String tipo = source.Configs[Constants.INTERFACE_INFORME_RECEPCION].GetString(Constants.INTERFACE_INFORME_RECEPCION_TIPO);
            String OrderCompany = source.Configs[Constants.INTERFACE_INFORME_RECEPCION].GetString(Constants.INTERFACE_INFORME_RECEPCION_ORDER_COMPANY);

            List<tblInformeRecepcion> informes = serviceInformeRecepcion.FindInformes(emplazamiento, almacen, tipo);
            List<InformeRecepcionDTO> informeRecepcionDTOList = null;
            InformeRecepcionJson informeRecepcionJson = null;

            /* Obtenemos usuario y contraseña del archivo para el servicio Rest */
            String urlPath = String.Empty;
            String user = source.Configs[Constants.BASIC_AUTH].Get(Constants.USER);
            String pass = source.Configs[Constants.BASIC_AUTH].Get(Constants.PASS);
            Console.WriteLine("Usuario del Servicio Rest: " + user);

            /* Obtenemos la URL del archivo */
            String url = source.Configs[INTERFACE + "." + Constants.URLS].GetString(Constants.INTERFACE_INFORME_RECEPCION_URL);

            int count = 0;
            int countError = 0;
            // int? tipoMensaje = 0;
            int? tipoProceso = source.Configs[INTERFACE].GetInt(Constants.NUMERO_INTERFACE);
            int codigoCliente = source.Configs[INTERFACE].GetInt(Constants.NUMERO_CLIENTE_INTERFACE_INFORME_RECEPCION);
            Console.WriteLine("Codigo de interface: " + tipoProceso);

            foreach (tblInformeRecepcion informe in informes)
            {
                informeRecepcionDTOList = InformeRecepcionUtils.MappingInforme(informe, OrderCompany);
                informeRecepcionJson = new InformeRecepcionJson(informeRecepcionDTOList);
                var json = InformeRecepcionUtils.ObjectToJson(informeRecepcionJson);
                // SEND REQUEST
                Boolean result = InformeRecepcionUtils.SendRequestPost(url, user, pass, json);

                // Este es el ID que va, no lo ejecutamos con ese ID para que no elimine nada, por eso lo pisamos con 1
                int? id = informe.irec_proc_id;
                if (result)
                {
                    id = 1;                    
                    ObjectParameter error = new ObjectParameter("error", typeof(String));
                    int salida = serviceInformeRecepcion.CallProcedureArchivarInformeRecepcion(id, error);
                    count++;
                }
                else
                {
                    id = 1;
                    ObjectParameter error = new ObjectParameter("error", typeof(String));
                    String mensaje = String.Empty;
                    int salida = serviceInformeRecepcion.CallProcedureInformarEjecucion(id, mensaje, error);
                    countError++;
                }

            }

            Console.WriteLine("Finalizó el proceso de actualización de Recepciones");

            /* Agregamos datos faltantes de la tabla de procesos */
            Console.WriteLine("Preparamos los datos a actualizar en BIANCHI_PROCESS");
            process.fin = DateTime.Now;
            process.fecha_ultima = lastTime;
            process.cant_lineas = count;
            process.estado = Constants.ESTADO_OK;
            Console.WriteLine("Fecha_fin: " + process.fin);
            Console.WriteLine("Cantidad de Recepciones procesadas OK: " + process.cant_lineas);
            Console.WriteLine("Cantidad de Recepciones procesadas con ERROR: " + countError);
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
