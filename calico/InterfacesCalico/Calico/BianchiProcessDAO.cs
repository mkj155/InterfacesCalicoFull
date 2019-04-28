using Calico.Persistencia;
using System.Data.Entity;

namespace Calico
{
    class BianchiProcessDAO : Dao<BIANCHI_PROCESS>
    {
        public void delete(int id)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                BIANCHI_PROCESS obj = new BIANCHI_PROCESS { id = id };
                context.BIANCHI_PROCESS.Attach(obj);
                context.BIANCHI_PROCESS.Remove(obj);
                context.SaveChanges();
            }
        }

        public DbSet<BIANCHI_PROCESS> findAll()
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                /* Obtengo todos los registros de la tabla de esta manera */
                var rows = context.Set<BIANCHI_PROCESS>();
                return rows;
            }
        }

        public BIANCHI_PROCESS findById(int id)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                return context.BIANCHI_PROCESS.Find(id);
            }
        }

        public void save(BIANCHI_PROCESS obj)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                context.BIANCHI_PROCESS.Add(obj);
                context.SaveChanges();
            }
        }

        public void update(BIANCHI_PROCESS obj)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                var result = context.BIANCHI_PROCESS.Find(obj.id);
                if (result == null) return;
                context.Entry(result).CurrentValues.SetValues(obj);
                context.SaveChanges();
            }
        }
    }
}
