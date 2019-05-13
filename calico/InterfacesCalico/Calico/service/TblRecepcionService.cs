using Calico.DAOs;
using Calico.persistencia;
using System;
using System.Collections.Generic;
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

        public Boolean IgnoreRegister(String emplaz, String alm, String cod, String numero)
        {
            int count = dao.CountByFields(emplaz, alm, cod, numero);

            if(count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void examplePersist(String empl,String alm,String cod,String num,String comp)
        {
            if (!IgnoreRegister(empl, alm, cod, num))
            {
                dao.examplePersist(empl, alm, cod, num, comp);
            }
            
        }

    }
}
