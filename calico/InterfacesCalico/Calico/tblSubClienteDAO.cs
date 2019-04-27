using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico
{
    class tblSubClienteDAO : Dao<tblSubCliente>
    {
        public void delete(int id)
        {
            using (calicoEntities context = new calicoEntities())
            {
                tblSubCliente obj = new tblSubCliente { subc_proc_id = id };
                context.tblSubCliente.Attach(obj);
                context.tblSubCliente.Remove(obj);
                context.SaveChanges();
            }
        }

        public DbSet<tblSubCliente> findAll()
        {
            using (calicoEntities context = new calicoEntities())
            {
                /* Obtengo todos los registros de la tabla de esta manera */
                var rows = context.Set<tblSubCliente>();
                return rows;
            }
        }

        public tblSubCliente findById(int id)
        {
            using (calicoEntities context = new calicoEntities())
            {
                return context.tblSubCliente.Find(id);
            }
        }

        public void save(tblSubCliente obj)
        {
            using (calicoEntities context = new calicoEntities())
            {
                context.tblSubCliente.Add(obj);
                context.SaveChanges();
            }
        }

        public void update(tblSubCliente obj)
        {
            using (calicoEntities context = new calicoEntities())
            {
                var result = context.tblSubCliente.Find(obj.subc_proc_id);
                if (result == null) return;
                context.Entry(result).CurrentValues.SetValues(obj);
                context.SaveChanges();
            }
        }
    }
}
