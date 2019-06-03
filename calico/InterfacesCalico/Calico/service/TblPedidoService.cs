using Calico.DAOs;
using Calico.persistencia;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Calico.service
{
    class TblPedidoService
    {
        TblPedidoDAO dao = new TblPedidoDAO();

        public void Delete(int id)
        {
            dao.Delete(id);
        }

        public DbSet<tblPedido> FindAll()
        {
            return dao.FindAll();
        }

        public tblPedido FindById(int id)
        {
            return dao.FindById(id);
        }

        public bool Save(tblPedido obj) 
        {
            return dao.Save(obj);
        }

        public void Update(tblPedido obj)
        {
            dao.Update(obj);
        }

        public int CallProcedure(int? tipoProceso, int? tipoMensaje)
        {
            return dao.CallProcedure(tipoProceso, tipoMensaje);
        }

        public Boolean IsAlreadyProcess(String alm, String tipo, String letra,String sucursal,decimal numero)
        {
            return dao.IsAlreadyProcess(alm,tipo,letra,sucursal,numero);
        }

    }
}
