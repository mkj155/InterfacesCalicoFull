using Calico.persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.DAOs
{
    public class CommonDAO
    {
        public int CallProcedure(int? tipoProceso, int? tipoMensaje)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                return context.INTERFAZ_CrearProceso(tipoProceso, tipoMensaje);
            }
        }

    }
}
