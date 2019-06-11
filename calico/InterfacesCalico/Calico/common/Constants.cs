using System;

namespace Calico.common
{
    public class Constants
    {
        /* INTERFACES */
        public const String NUMERO_INTERFACE = "NumeroInterface";
        public const String FECHA_DEFAULT = "1973/01/01";
        public const String NUMERO_CLIENTE = "NumeroCliente";
        public const String INTERFACE_EMPLAZAMIENTO = "Emplazamiento";
        public const String ALMACEN = "Almacen";
        public const String INTERFACE_COMPANIA = "Compania";
        public const String INTERFACE_SUCURSAL = "Sucursal";
        public const String INTERFACE_CLIENTE = "Cliente";
        public const String INTERFACE_TIPO = "Tipo";
        public const String INTERFACE_TIPO_PROCESO = "Tipo";
        // CLIENTES
        public const String INTERFACE_CLIENTES = "Clientes";
        public const String NUMERO_CLIENTE_INTERFACE_CLIENTE = "NumeroCliente";
        // RECEPCION
        public const String INTERFACE_RECEPCION = "Recepcion";
        public const String INTERFACE_RECEPCION_URL = "ReceptionURL";
        public const String INTERFACE_RECEPCION_EMPLAZAMIENTO = "Emplazamiento";
        public const String INTERFACE_RECEPCION_CODIGO = "Codigo";
        public const String INTERFACE_RECEPCION_COMPANIA = "Compania";
        public const String NUMERO_CLIENTE_INTERFACE_RECEPCION = "NumeroCliente";
        // INFORME_RECEPCION
        public const String INTERFACE_INFORME_RECEPCION = "InformeRecepcion";
        public const String INTERFACE_INFORME_RECEPCION_URL = "InformeRecepcionURL";
        public const String NUMERO_CLIENTE_INTERFACE_INFORME_RECEPCION = "NumeroCliente";
        public const String INTERFACE_INFORME_RECEPCION_ORDER_COMPANY = "OrderCompany";
        public const String INTERFACE_INFORME_RECEPCION_RECEIPTS_VERSION = "ReceiptsVersion";
        public const String INTERFACE_INFORME_RECEPCION_TIPO = "Tipo";
        public const String INTERFACE_REPEATING_REQUEST = "Repeating Requests";
        public const String INTERFACE_RECEIPT_DOCUMENT = "ReceiptDocument";
        public const String INTERFACE_ORDER_TYPE = "orderType_ST";
        // PEDIDOS
        public const String INTERFACE_PEDIDOS = "Pedidos";
        public const String INTERFACE_PEDIDOS_TIPO_PEDIDO = "tipoPedido";
        public const String INTERFACE_PEDIDOS_LETRA = "Letra";
        public const String INTERFACE_PEDIDOS_URL = "PedidosURL";
        public const String INTERFACE_PEDIDOS_URL_POST = "PedidosURL_POST";
        
		// INFORME PEDIDOS
        public const String INTERFACE_INFORME_PEDIDOS = "InformePedido";
        public const String INTERFACE_INFORME_PEDIDO_URL = "InformePedidoURL";
        public const String INTERFACE_INFORME_PEDIDO_LAST_STATUS = "laststatus";
        public const String INTERFACE_INFORME_PEDIDO_NEXT_STATUS = "nextstatus";
        public const String INTERFACE_INFORME_PEDIDO_ORDER_COMPANY = "OrderCompany";
        public const String INTERFACE_INFORME_PEDIDO_P554211I_VERSION = "P554211I_Version";
        public const String INTERFACE_PEDIDOS_FROM_STATUS = "fromStatus";
        public const String INTERFACE_PEDIDOS_TO_STATUS = "toStatus";

        // JSON
        public const String JSON_PREFIX = "fs_DATABROWSE_";
        public const String JSON_SUBFIX_MLNM = "F0111";
        public const String JSON_SUBFIX_TAX = "F0101";
        public const String JSON_SUBFIX_RECEPTION = "V554211";
        public const String JSON_SUBFIX_PEDIDO = "V554211";
        public const String JSON_TAG_DATA = "data";
        public const String JSON_TAG_GRIDDATA = "gridData";
        public const String JSON_TAG_ROWSET = "rowset";

        // ARGUMENTOS ENTRADA
        public const String MUST_LOG = "/l";

        // METHOD REST
        public const String METHOD_GET = "GET";
        public const String METHOD_POST = "POST";

        // ESTADOS DE EJECUCION
        public const String ESTADO_EN_CURSO = "EN_CURSO";
        public const String ESTADO_OK = "OK";
        public const String ESTADO_ERROR = "ERROR";

        // ARCHIVO CONFIGURACION EXTERNO
        public const String URLS = "URLs";
        public const String MLNM = "MLNM";
        public const String TAX = "TAX";
        public const String USER = "user";
        public const String PASS = "pass";
        public const String BASIC_AUTH = "BasicAuth";
        public const String PARAM_FECHA = "{fecha}";
        public const String PARAM_TIPO_PEDIDO = "{tipoPedido}";

        // COLUMNAS
        public const String COLUMN_AT1 = "AT1";   // Sch Typ
        public const String COLUMN_AN8 = "AN8";   // Address Number
        public const String COLUMN_TAX = "TAX";   // Tax Id
        public const String COLUMN_ALKY = "ALKY"; // Long Address
        public const String COLUMN_AC29 = "AC29"; // CC 29
        public const String COLUMN_ALPH = "ALPH"; // Alpha Name
        public const String COLUMN_MCU = "MCU";   // Business Unit
        public const String COLUMN_AC01 = "AC01"; // Cat Code 1
        public const String COLUMN_MLNM = "MLNM"; // RAZON SOCIAL 

        //ERRORES
        public const String NOT_DATA_FOUND = "No se recibieron datos del Rest Service";
        public const String FAILED_CALL_REST = "Fallo el llamado al Rest Service";
        public const String FAILED_LOAD_FILE = "No se pudo cargar el archivo de configuración";
        public const String FAILED_LOAD_DATES = "La fecha de BIANCHI_PROCESS es NULL y no se indico fecha como parametro, no se ejecutara el proceso";
        public const String FAILED_GETTING_DATA = "No se encontraron Pedidos para procesar";
        public const String FAILED_CALL_REST_PEDIDO = "Fallo el llamado Rest Service de Pedidos tipo {0}";
        
    }
}
