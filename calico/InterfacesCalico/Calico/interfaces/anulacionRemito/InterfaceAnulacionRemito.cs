using Calico.common;
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

namespace Calico.interfaces.informePedido
{
    class InterfaceAnulacionRemito : InterfaceGeneric
    {

        private const String INTERFACE = Constants.INTERFACE_ANULACION_REMITO;

        private BianchiService service = new BianchiService();
        private TblInformePedidoService serviceInformePedido = new TblInformePedidoService();

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
            String emplazamiento = source.Configs[INTERFACE].GetString(Constants.INTERFACE_EMPLAZAMIENTO);
            String orderCompany = source.Configs[INTERFACE].GetString(Constants.INTERFACE_INFORME_PEDIDO_ORDER_COMPANY);
            String lastStatus = source.Configs[INTERFACE].GetString(Constants.INTERFACE_INFORME_PEDIDO_LAST_STATUS);
            String nextStatus = source.Configs[INTERFACE].GetString(Constants.INTERFACE_INFORME_PEDIDO_NEXT_STATUS);
            String version = source.Configs[INTERFACE].GetString(Constants.INTERFACE_INFORME_PEDIDO_P554211I_VERSION);
            int tipoProceso = source.Configs[INTERFACE].GetInt(Constants.INTERFACE_TIPO_PROCESO);

            var almacenes = source.Configs[INTERFACE + "." + Constants.ALMACEN].GetValues();
            var tipos = source.Configs[INTERFACE + "." + Constants.INTERFACE_TIPO].GetValues();

            List<tblInformePedido> informes = serviceInformePedido.FindInformes(emplazamiento, almacenes, tipos, tipoProceso);
            List<InformePedidoJson> jsonList = null;

            /* Obtenemos usuario y contraseña del archivo para el servicio Rest */
            String urlPath = String.Empty;
            String user = source.Configs[Constants.BASIC_AUTH].Get(Constants.USER);
            String pass = source.Configs[Constants.BASIC_AUTH].Get(Constants.PASS);
            Console.WriteLine("Usuario del Servicio Rest: " + user);

            /* Obtenemos la URL del archivo */
            String url = source.Configs[INTERFACE + "." + Constants.URLS].GetString(Constants.INTERFACE_ANULACION_REMITO_URL);

            int count = 0;
            int countError = 0;
            Boolean callArchivar;
            //int? tipoProceso = source.Configs[INTERFACE].GetInt(Constants.NUMERO_INTERFACE);
            //int codigoCliente = source.Configs[INTERFACE].GetInt(Constants.NUMERO_CLIENTE_INTERFACE_INFORME_RECEPCION);
            //Console.WriteLine("Codigo de interface: " + tipoProceso);

            foreach (tblInformePedido informe in informes)
            {
                callArchivar = true;
                String orderType = String.Empty;
                if (!String.IsNullOrWhiteSpace(informe.ipec_letra))
                {
                    orderType = source.Configs[INTERFACE + "." + Constants.INTERFACE_PEDIDOS_LETRA].GetString(informe.ipec_letra.Trim());
                }
                jsonList = InformePedidoUtils.MappingInforme(informe, orderCompany, orderType, lastStatus,nextStatus,version);

                if (jsonList.Any())
                {
                    Console.WriteLine("Se llevara a cabo el envio al servicio REST de los detalles de la cabecera: " + informe.ipec_proc_id);
                    foreach (InformePedidoJson json in jsonList)
                    {
                        var jsonString = InformePedidoUtils.JsonToString(json);
                        Console.WriteLine("Se enviara el siguiente Json al servicio REST: ");
                        Console.WriteLine(jsonString);
                        /* Send request */
                        if (!(InformePedidoUtils.SendRequestPost(url, user, pass, jsonString)))
                        {
                            Console.WriteLine("Se llamara al procedure para informar el error");
                            serviceInformePedido.CallProcedureInformarEjecucion(informe.ipec_proc_id, InformePedidoUtils.LAST_ERROR, new ObjectParameter("error", typeof(String)));
                            callArchivar = false;
                            countError++;
                        }
                        else
                        {
                            Console.WriteLine("El servicio REST retorno OK:");
                            Console.WriteLine(jsonString);
                            count++;
                        }
                    }

                    if (callArchivar)
                    {
                        Console.WriteLine("Se llamara al procedure para archivar el informe");
                        serviceInformePedido.CallProcedureArchivarInformePedido(informe.ipec_proc_id, new ObjectParameter("error", typeof(String)));
                    }

                }
                else
                {
                    Console.WriteLine("No se encontraron detalles para la cabecera: " + informe.ipec_proc_id);
                }

            }

            Console.WriteLine("Finalizó el proceso de envio de anulaciones");

            /* Agregamos datos faltantes de la tabla de procesos */
            Console.WriteLine("Preparamos los datos a actualizar en BIANCHI_PROCESS");
            process.fin = DateTime.Now;
            process.fecha_ultima = process.inicio;
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
