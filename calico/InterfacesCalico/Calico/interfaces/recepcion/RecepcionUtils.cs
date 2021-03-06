﻿using Calico.common;
using Calico.persistencia;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nini.Config;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Calico.interfaces.recepcion
{
    class RecepcionUtils
    {
        public String BuildUrl(String urlParam, String fecha)
        {
            Dictionary<String, String> dictionary = new Dictionary<string, string>();
            dictionary.Add(Constants.PARAM_FECHA, fecha);
            return Utils.BuildUrl(urlParam, dictionary);
        }

        public List<ReceptionDTO> MappingJsonRecepcion(String myJsonString)
        {
            var jc = JsonConvert.DeserializeObject<JObject>(myJsonString);

            JArray rowset = jc.Value<JObject>(Constants.JSON_PREFIX + Constants.JSON_SUBFIX_RECEPTION)
                                 .Value<JObject>(Constants.JSON_TAG_DATA)
                                 .Value<JObject>(Constants.JSON_TAG_GRIDDATA)
                                 .Value<JArray>(Constants.JSON_TAG_ROWSET);

            if (rowset != null && rowset.Count > 0)
            {
                return rowset.ToObject<List<ReceptionDTO>>() as List<ReceptionDTO>;
            }

            return new List<ReceptionDTO>();
        }

        public void MappingReceptionDTORecepcion(List<ReceptionDTO> receptionDTOList, Dictionary<String, tblRecepcion> dictionary, String emplazamiento)
        {
            foreach(ReceptionDTO receptionDTO in receptionDTOList)
            {
                tblRecepcion recepcion = null;
                dictionary.TryGetValue(receptionDTO.F4201_DOCO, out recepcion);
                if (recepcion == null)
                {
                    /* CABEZERA */
                    recepcion = fillCabezera(receptionDTO, emplazamiento);
                    /* DETALLE */
                    tblRecepcionDetalle detalle = fillDetalle(receptionDTO);
                    recepcion.tblRecepcionDetalle.Add(detalle);
                    dictionary.Add(receptionDTO.F4201_DOCO, recepcion);
                }
                else
                {
                    /* DETALLE */
                    tblRecepcionDetalle detalle = fillDetalle(receptionDTO);
                    recepcion.tblRecepcionDetalle.Add(detalle);
                }
            }
        }

        private tblRecepcion fillCabezera(ReceptionDTO receptionDTO, String emplazamiento)
        {
            tblRecepcion recepcion = new tblRecepcion();
            recepcion.recc_contacto = String.Empty;
            recepcion.recc_despacho = String.Empty;
            recepcion.recc_ordenCompra = String.Empty;
            recepcion.recc_motivoDevolucion = String.Empty;
            recepcion.recc_observaciones = String.Empty;
            recepcion.recc_emplazamiento = emplazamiento;
            recepcion.recc_trec_codigo = receptionDTO.F4201_DCTO;
            recepcion.recc_numero = receptionDTO.F4201_DOCO;

            if (!String.IsNullOrWhiteSpace(receptionDTO.F4201_OPDJ))
            {
                string result = DateTime.ParseExact(receptionDTO.F4201_OPDJ, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                recepcion.recc_fechaEntrega = Utils.ParseDate(result, "yyyy/MM/dd");
            }
            else
            {
                recepcion.recc_fechaEntrega = Utils.ParseDate(Constants.FECHA_DEFAULT, "yyyy/MM/dd");
            }

            recepcion.recc_proveedor = !String.IsNullOrWhiteSpace(receptionDTO.F4211_MCU) ? receptionDTO.F4211_MCU.Trim() : String.Empty;

            // VERY HARDCODE
            recepcion.recc_fechaEmision = Utils.ParseDate(Constants.FECHA_DEFAULT, "yyyy/MM/dd");

            return recepcion;
        }

        private tblRecepcionDetalle fillDetalle(ReceptionDTO receptionDTO)
        {
            tblRecepcionDetalle detalle = new tblRecepcionDetalle();
            detalle.recd_serie = String.Empty;
            detalle.recd_linea = !String.IsNullOrWhiteSpace(receptionDTO.F4211_LNID) ? Convert.ToInt64(Convert.ToDouble(receptionDTO.F4211_LNID)) * 1000 : 0;
            detalle.recd_lineaPedido = 0;
            detalle.recd_lote = !String.IsNullOrWhiteSpace(receptionDTO.F4211_LOTN) ? receptionDTO.F4211_LOTN.Trim() : String.Empty;
            detalle.recd_cantidad = !String.IsNullOrWhiteSpace(receptionDTO.F4211_UORG) ? Convert.ToInt64(Convert.ToDouble(receptionDTO.F4211_UORG)) : 0;
            detalle.recd_compania = !String.IsNullOrWhiteSpace(receptionDTO.F4211_SRP1) ? receptionDTO.F4211_SRP1.Trim() : String.Empty;

            if (!String.IsNullOrWhiteSpace(receptionDTO.F4211_LITM) && receptionDTO.F4211_LITM.Length > 15)
            {
                // VERY HARDCODE
                detalle.recd_producto = "99999999999999";
            }
            else
            {
                detalle.recd_producto = receptionDTO.F4211_LITM;
            }

            if (!String.IsNullOrWhiteSpace(receptionDTO.F4108_MMEJ))
            {
                string result = DateTime.ParseExact(receptionDTO.F4108_MMEJ, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                detalle.recd_fechaVencimiento = Utils.ParseDate(result, "yyyy/MM/dd");
            }
            else
            {
                detalle.recd_fechaVencimiento = Utils.ParseDate(Constants.FECHA_DEFAULT, "yyyy/MM/dd");
            }

            // VERY HARDCODE
            detalle.recd_numeroPedido = "0";

            return detalle;
        }

    }
}
