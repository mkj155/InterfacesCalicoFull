using Calico.common;
using Calico.interfaces.pedido;
using Calico.persistencia;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Calico.interfaces.pedidos
{
    class PedidoUtils
    {
        public String BuildUrl(String urlParam, String param, String value)
        {
            Dictionary<String, String> dictionary = new Dictionary<string, string>();
            dictionary.Add(param, value);
            return Utils.BuildUrl(urlParam, dictionary);
        }

        public List<PedidoDTO> MappingJsonRecepcion(String myJsonString)
        {
            var jc = JsonConvert.DeserializeObject<JObject>(myJsonString);

            JArray rowset = jc.Value<JObject>(Constants.JSON_PREFIX + Constants.JSON_SUBFIX_PEDIDO)
                                 .Value<JObject>(Constants.JSON_TAG_DATA)
                                 .Value<JObject>(Constants.JSON_TAG_GRIDDATA)
                                 .Value<JArray>(Constants.JSON_TAG_ROWSET);

            if (rowset != null && rowset.Count > 0)
            {
                return rowset.ToObject<List<PedidoDTO>>() as List<PedidoDTO>;
            }

            return new List<PedidoDTO>();
        }

        public void MappingReceptionDTORecepcion(List<PedidoDTO> pedidoDTOList, Dictionary<int, tblPedido> dictionary, String emplazamiento, String almacen, String compania, String letra, String sucursal, String cliente, String tipoPedido)
        {
            foreach(PedidoDTO pedidoDTO in pedidoDTOList)
            {
                tblPedido pedido = null;
                dictionary.TryGetValue(pedidoDTO.F4201_DOCO, out pedido);
                if (pedido == null)
                {
                    /* CABEZERA */
                    pedido = fillCabezera(pedidoDTO, emplazamiento, almacen, letra, sucursal, cliente, tipoPedido);
                    /* DETALLE */
                    tblPedidoDetalle detalle = fillDetalle(pedidoDTO, compania);
                    pedido.tblPedidoDetalle.Add(detalle);
                    dictionary.Add(pedidoDTO.F4201_DOCO, pedido);
                }
                else
                {
                    /* DETALLE */
                    tblPedidoDetalle detalle = fillDetalle(pedidoDTO, compania);
                    pedido.tblPedidoDetalle.Add(detalle);
                }
            }
        }

        private tblPedido fillCabezera(PedidoDTO pedidoDTO, String emplazamiento, String almacen, String letra, String sucursal, String cliente, String tipoPedido)
        {
            tblPedido pedido = new tblPedido();
            pedido.pedc_emplazamiento = emplazamiento;
            pedido.pedc_almacen = almacen;
            pedido.pedc_tped_codigo = tipoPedido;
            pedido.pedc_letra = letra;
            pedido.pedc_sucursal = sucursal;
            pedido.pedc_numero = pedidoDTO.F4201_DOCO;

            if (!String.IsNullOrWhiteSpace(pedidoDTO.F4201_OPDJ))
            {
                string result = DateTime.ParseExact(pedidoDTO.F4201_OPDJ, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                pedido.pedc_fechaEntrega = Utils.ParseDate(result, "yyyy/MM/dd");
            }
            else
            {
                pedido.pedc_fechaEntrega = Utils.ParseDate(Constants.FECHA_DEFAULT, "yyyy/MM/dd");
            }

            pedido.pedc_cliente = cliente;
            pedido.pedc_destinatario = !String.IsNullOrWhiteSpace(pedidoDTO.F4211_MCU) ? pedidoDTO.F4211_MCU.Trim() : String.Empty;
            pedido.pedc_referenciaA = String.Empty; /* Revisar por parte de Jorge */
            pedido.pedc_referenciaB = String.Empty; /* Revisar por parte de Jorge */

            pedido.pedc_areaMuelle = String.Empty;
            pedido.pedc_centroCosto = String.Empty;
            pedido.pedc_codigoPostal = String.Empty;
            pedido.pedc_contraRembolso = 0;
            pedido.pedc_domicilio = String.Empty;
            pedido.pedc_entregaParcial = false;
            pedido.pedc_fechaEmision = Utils.ParseDate(Constants.FECHA_DEFAULT, "yyyy/MM/dd");
            pedido.pedc_importeFactura = 0;
            pedido.pedc_localidad = String.Empty;
            pedido.pedc_numeroRuteo = 0;
            pedido.pedc_observaciones = String.Empty;
            pedido.pedc_pais = String.Empty;
            pedido.pedc_prioridad = 0;
            pedido.pedc_provincia = String.Empty;
            pedido.pedc_razonSocial = String.Empty;

            return pedido;
        }

        private tblPedidoDetalle fillDetalle(PedidoDTO pedidoDTO, String compania)
        {
            tblPedidoDetalle detalle = new tblPedidoDetalle();
            detalle.pedd_linea = !String.IsNullOrWhiteSpace(pedidoDTO.F4211_LNID) ? Convert.ToDecimal(pedidoDTO.F4211_LNID) : 0;
            detalle.pedd_compania = compania;
            detalle.pedd_producto = !String.IsNullOrWhiteSpace(pedidoDTO.F4211_LITM) ? pedidoDTO.F4211_LITM : String.Empty;
            detalle.pedd_lote = !String.IsNullOrWhiteSpace(pedidoDTO.F4211_LOTN) ? pedidoDTO.F4211_LOTN : String.Empty;
            detalle.pedd_cantidad = !String.IsNullOrWhiteSpace(pedidoDTO.F4211_UORG) ? Convert.ToDecimal(pedidoDTO.F4211_UORG) : 0;

            detalle.pedd_despachoParcial = false;
            detalle.pedd_epro_codigo = String.Empty;
            detalle.pedd_loteUnico = false;
            detalle.pedd_serie = String.Empty;
          
            return detalle;
        }

    }
}
