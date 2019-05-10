using Calico.DAOs;
using Calico.persistencia;
using System.Data.Entity;

namespace Calico.service
{
    class TblRecepcionService
    {
        TblRecepcionDAO dao = new TblRecepcionDAO();
        public void Delete(int id)
        {
            dao.Delete(id);
        }

        public DbSet<tblRecepcion> FindAll()
        {
            return dao.FindAll();
        }

        public tblRecepcion FindById(int id)
        {
            return dao.FindById(id);
        }

        public void Save(tblRecepcion obj)
        {
            dao.Save(obj);
        }

        public void Update(tblRecepcion obj)
        {
            dao.Update(obj);
        }

        public int CallProcedure(int? tipoProceso, int? tipoMensaje)
        {
            return dao.CallProcedure(tipoProceso, tipoMensaje);
        }

    }
}
