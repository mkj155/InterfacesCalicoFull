using Calico.persistencia;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.DAOs
{
    class TblRecepcionDAO : CommonDAO, Dao<tblRecepcion>
    {
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public DbSet<tblRecepcion> FindAll()
        {
            throw new NotImplementedException();
        }

        public tblRecepcion FindById(int id)
        {
            throw new NotImplementedException();
        }

        public void Save(tblRecepcion obj)
        {
            throw new NotImplementedException();
        }

        public void Update(tblRecepcion obj)
        {
            throw new NotImplementedException();
        }
    }
}
