using Calico.DAOs;
using Calico.persistencia;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.service
{
    class TblInformeRecepcionService
    {
        TblInformeRecepcionDAO dao = new TblInformeRecepcionDAO();

        public int CallProcedureArchivarInformeRecepcion(int? id, ObjectParameter error)
        {
            return dao.CallProcedureArchivarInformeRecepcion(id, error);
        }

        public int CallProcedureInformarEjecucion(int? id, string mensaje, ObjectParameter error)
        {
            return dao.CallProcedureInformarEjecucion(id, mensaje, error);
        }

        public List<tblInformeRecepcion> FindInformes(String emplazamiento, String almacen, String tipo)
        {
            return dao.FindInformes(emplazamiento, almacen, tipo);
        }

    }
}
