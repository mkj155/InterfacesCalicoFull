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
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                using (CalicoEntities context = new CalicoEntities())
                {
                    BIANCHI_PROCESS obj = new BIANCHI_PROCESS { id = id };
                    context.BIANCHI_PROCESS.Attach(obj);
                    context.BIANCHI_PROCESS.Remove(obj);
                    context.SaveChanges();
                }

                scope.Complete();
            }
        }

        public DbSet<BIANCHI_PROCESS> findAll()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                DbSet<BIANCHI_PROCESS> rows;

                using (CalicoEntities context = new CalicoEntities())
                {
                    /* Obtengo todos los registros de la tabla de esta manera */
                    rows = context.Set<BIANCHI_PROCESS>();
                }

                scope.Complete();

                return rows;
            }
        }

        public BIANCHI_PROCESS findById(int id)
        {
            BIANCHI_PROCESS process;

            using (CalicoEntities entities = new CalicoEntities())
            using (DbContextTransaction scope = entities.Database.BeginTransaction())
            {
                //Lock the table during this transaction
                entities.Database.ExecuteSqlCommand("SELECT TOP 0 NULL FROM BIANCHI_PROCESS WITH (TABLOCKX)");

                //Do your work with the locked table here...
                process = entities.BIANCHI_PROCESS.Find(id);

                //Complete the scope here to commit, otherwise it will rollback
                //The table lock will be released after we exit the TransactionScope block
                scope.Commit();
                
                return process;
            }
        }

        public void save(BIANCHI_PROCESS obj)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
            {
                using (CalicoEntities context = new CalicoEntities())
                {
                    context.BIANCHI_PROCESS.Add(obj);
                    context.SaveChanges();
                }

                scope.Complete();
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

    }
}
