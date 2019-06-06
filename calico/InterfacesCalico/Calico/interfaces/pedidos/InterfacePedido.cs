using Calico.common;
using Calico.interfaces.pedidos;
using Calico.persistencia;
using Calico.service;
using InterfacesCalico.generic;
using Nini.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calico.interfaces.pedido
{
    class InterfacePedido : InterfaceGeneric
    {
        private const String INTERFACE = Constants.INTERFACE_PEDIDOS;

        private BianchiService service = new BianchiService();
        private TblPedidoService servicePedido = new TblPedidoService();
        private PedidoUtils pedidoUtils = new PedidoUtils();

        public bool ValidateDate() => true;

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

            /* Obtenemos la fecha */
            if (Utils.IsInvalidateDates(dateTime, process.fecha_ultima))
            {
                service.finishProcessByError(process, Constants.FAILED_LOAD_DATES, INTERFACE);
                return false;
            }
            DateTime lastTime = Utils.GetDateToProcess(dateTime, process.fecha_ultima);

            /* Convierto DateTime a String formato YYYYMMDD */
            String lastStringTime = lastStringTime = Utils.ConvertDateTimeInString(lastTime);

            /* Cargamos archivo con parametros propios para cada interface */
            Console.WriteLine("Cargamos archivo de configuracion");
            IConfigSource source = null;
            try
            {
                source = new IniConfigSource("calico_config.ini");
            }
            catch(Exception)
            {
                service.finishProcessByError(process, Constants.FAILED_LOAD_FILE, INTERFACE);
                return false;
            }

            // INICIO BUSQUEDA DE DATOS
            String numeroInterfaz = source.Configs[INTERFACE].GetString(Constants.NUMERO_INTERFACE);
            String emplazamiento = source.Configs[INTERFACE].GetString(Constants.INTERFACE_EMPLAZAMIENTO);
            String almacen = source.Configs[INTERFACE].GetString(Constants.INTERFACE_ALMACEN);
            String compania = source.Configs[INTERFACE].GetString(Constants.INTERFACE_COMPANIA);
            String sucursal = source.Configs[INTERFACE].GetString(Constants.INTERFACE_SUCURSAL);
            String cliente = source.Configs[INTERFACE].GetString(Constants.INTERFACE_CLIENTE);
            String fromStatus = source.Configs[INTERFACE].GetString(Constants.INTERFACE_PEDIDOS_FROM_STATUS);
            String toStatus = source.Configs[INTERFACE].GetString(Constants.INTERFACE_PEDIDOS_TO_STATUS);

            /* Obtenemos usuario y contraseña del archivo para el servicio Rest */
            String urlPath = String.Empty;
            String user = source.Configs[Constants.BASIC_AUTH].Get(Constants.USER);
            String pass = source.Configs[Constants.BASIC_AUTH].Get(Constants.PASS);
            Console.WriteLine("Usuario del Servicio Rest: " + user);
            
            /* Obtenemos la URL del archivo */
            String url = source.Configs[INTERFACE + "." + Constants.URLS].GetString(Constants.INTERFACE_PEDIDOS_URL);

            /* Obtenemos los tipos de pedidos del archivo externo para llamar a la URL segun tipo */
            String[] URLkeys = source.Configs[INTERFACE + "." + Constants.INTERFACE_PEDIDOS_TIPO_PEDIDO].GetKeys();

            /* TEST */
            String urlPost = source.Configs[INTERFACE + "." + Constants.URLS].GetString(Constants.INTERFACE_PEDIDOS_URL_POST);

            int countOKPedido = 0;
            int countErrorPedido = 0;
            int countAlreadyProcessPedido = 0;
            int? tipoMensaje = 0;
            int? tipoProceso = source.Configs[INTERFACE].GetInt(Constants.NUMERO_INTERFACE);
            int codigoCliente = source.Configs[INTERFACE].GetInt(Constants.NUMERO_CLIENTE);
            Console.WriteLine("Codigo de interface: " + tipoProceso);
            String urlWithDate = pedidoUtils.BuildUrl(url, Constants.PARAM_FECHA, lastStringTime);

            /* Mapping */
            List<PedidoDTO> pedidoDTO = null;
            Dictionary<int, tblPedido> dictionary = new Dictionary<int, tblPedido>();

            /* Armamos la URL con parametros */
            PedidoJson json = pedidoUtils.getJson(fromStatus, toStatus);
            var jsonString = pedidoUtils.JsonToString(json);
            Console.WriteLine("Se enviara el siguiente Json al servicio REST: ");
            Console.WriteLine(jsonString);
            Console.WriteLine("Se realiza el envio al servicio REST : " + urlPost);
            List<PedidoDTO> pedidos = pedidoUtils.SendRequestPost(urlPost, user, pass, jsonString);
            if (pedidos.Any())
            {   
                pedidoUtils.MappingPedidoDTOPedido(pedidoDTO, dictionary, emplazamiento, almacen, compania, sucursal, cliente,source);
                // Validamos si hay que insertar o descartar el pedido
                foreach (KeyValuePair<int, tblPedido> entry in dictionary)
                {
                    if (servicePedido.IsAlreadyProcess(almacen,entry.Value.pedc_tped_codigo, entry.Value.pedc_letra, sucursal, entry.Value.pedc_numero))
                    {
                        Console.WriteLine("El pedido " + entry.Value.pedc_numero + " ya fue tratado, no se procesara");
                        countAlreadyProcessPedido++;
                    }
                    // No está procesada! la voy a guardar
                    else
                    {
                        // LLamo al SP y seteo su valor a la cabecera y sus detalles
                        int recc_proc_id = servicePedido.CallProcedure(tipoProceso, tipoMensaje);
                        entry.Value.pedc_proc_id = recc_proc_id;
                        foreach (tblPedidoDetalle detalle in entry.Value.tblPedidoDetalle)
                        {
                            detalle.pedd_proc_id = recc_proc_id;
                        }

                        Console.WriteLine("Procesando pedido: " + entry.Value.pedc_numero);
                        if (servicePedido.Save(entry.Value)) countOKPedido++;
                        else countErrorPedido++;
                    }
                }
            }
            else
            {
                Console.WriteLine(Constants.FAILED_GETTING_DATA);
            }

            Console.WriteLine("Finalizó el proceso de actualización de Pedidos");

            /* Agregamos datos faltantes de la tabla de procesos */
            Console.WriteLine("Preparamos los datos a actualizar en BIANCHI_PROCESS");
            process.fin = DateTime.Now;
            process.fecha_ultima = lastTime;
            process.cant_lineas = countOKPedido;
            process.estado = Constants.ESTADO_OK;
            Console.WriteLine("Fecha_fin: " + process.fin);
            Console.WriteLine("Cantidad de pedidos procesados OK: " + process.cant_lineas);
            Console.WriteLine("Cantidad de pedidos procesados con ERROR: " + countErrorPedido);
            Console.WriteLine("Cantidad de pedidos evitados: " + countAlreadyProcessPedido);
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
