//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Calico.persistencia
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblPedidoDetalle
    {
        public int pedd_id { get; set; }
        public int pedd_proc_id { get; set; }
        public decimal pedd_linea { get; set; }
        public string pedd_compania { get; set; }
        public string pedd_producto { get; set; }
        public string pedd_lote { get; set; }
        public bool pedd_loteUnico { get; set; }
        public string pedd_serie { get; set; }
        public decimal pedd_cantidad { get; set; }
        public string pedd_epro_codigo { get; set; }
        public bool pedd_despachoParcial { get; set; }
    
        public virtual tblPedido tblPedido { get; set; }
    }
}
