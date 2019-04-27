using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico
{
    class BianchiProcessDAO : Dao<BIANCHI_PROCESS>
    {
        public void delete(int id)
        {
            using (calicoEntities context = new calicoEntities())
            {
                BIANCHI_PROCESS obj = new BIANCHI_PROCESS { id = id };
                context.BIANCHI_PROCESS.Attach(obj);
                context.BIANCHI_PROCESS.Remove(obj);
                context.SaveChanges();
            }
        }

        public DbSet<BIANCHI_PROCESS> findAll()
        {
            using (calicoEntities context = new calicoEntities())
            {
                /* Obtengo todos los registros de la tabla de esta manera */
                var rows = context.Set<BIANCHI_PROCESS>();
                return rows;
            }
        }

        public BIANCHI_PROCESS findById(int id)
        {
            using (calicoEntities context = new calicoEntities())
            {
                return context.BIANCHI_PROCESS.Find(id);
            }
        }

        public void save(BIANCHI_PROCESS obj)
        {
            using (calicoEntities context = new calicoEntities())
            {
                context.BIANCHI_PROCESS.Add(obj);
                context.SaveChanges();

            }
        }

        public void update(BIANCHI_PROCESS obj)
        {
            using (calicoEntities context = new calicoEntities())
            {
                var result = context.BIANCHI_PROCESS.Find(obj.id);
                if (result == null) return;
                context.Entry(result).CurrentValues.SetValues(obj);
                context.SaveChanges();
            }
        }
    }
}
