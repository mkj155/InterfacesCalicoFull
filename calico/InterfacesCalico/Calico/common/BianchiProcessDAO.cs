using Calico.Persistencia;
using System;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Calico.common
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
                    return context.Set<BIANCHI_PROCESS>();
                }

        }

        public BIANCHI_PROCESS findById(int id)
        {
            using (CalicoEntities entities = new CalicoEntities())
            {
                return entities.BIANCHI_PROCESS.Find(id);
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

        public bool updateEnCurso(string interfaz)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                var query = from BP in context.BIANCHI_PROCESS
                            where BP.interfaz == interfaz
                            select BP;
                var result = query.FirstOrDefault<BIANCHI_PROCESS>();
                if (result == null) return false;
                result.estado = Constants.ESTADO_EN_CURSO;
                context.Entry(result);
                context.SaveChanges();
                return true;
            }
        }

        public BIANCHI_PROCESS findByName(string interfaz)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                var query = from BP in context.BIANCHI_PROCESS
                            where BP.interfaz == interfaz
                            select BP;

                return query.FirstOrDefault<BIANCHI_PROCESS>();
            }
        }

        public bool validarSiPuedoProcesar(string interfaz)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                var result = context.BIANCHI_PROCESS.Where(bp => bp.interfaz == interfaz).FirstOrDefault<BIANCHI_PROCESS>();
                if (result == null) return true;
                return !Constants.ESTADO_EN_CURSO.Equals(result.estado);
            }
        }

        public DateTime? getProcessDate(string interfaz)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                var result = context.BIANCHI_PROCESS.Where(bp => bp.interfaz == interfaz).FirstOrDefault<BIANCHI_PROCESS>();
                return result.fecha_ultima;
            }
        }

        public void blockRow(int id, String interfaz)
        {
            using (CalicoEntities entities = new CalicoEntities())
            using (DbContextTransaction scope = entities.Database.BeginTransaction())
            {
                entities.Database.ExecuteSqlCommand("UPDATE BIANCHI_PROCESS SET interfaz = '" + interfaz + "' where id = " + id);
                scope.Commit();
            }
        }

    }
}
