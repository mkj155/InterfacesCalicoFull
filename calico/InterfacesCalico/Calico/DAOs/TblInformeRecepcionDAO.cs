using Calico.persistencia;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.DAOs
{
    class TblInformeRecepcionDAO : CommonDAO, Dao<tblInformeRecepcion>
    {
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public DbSet<tblInformeRecepcion> FindAll()
        {
            throw new NotImplementedException();
        }

        public tblInformeRecepcion FindById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Save(tblInformeRecepcion obj)
        {
            throw new NotImplementedException();
        }

        public void Update(tblInformeRecepcion obj)
        {
            throw new NotImplementedException();
        }
    }
}
